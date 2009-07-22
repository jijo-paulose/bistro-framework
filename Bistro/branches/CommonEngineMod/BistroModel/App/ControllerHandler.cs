using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Web;
using System.Collections;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Manages the state of a controller pre and post processing.
	/// </summary>
	public class ControllerHandler : ControllerHandlerBase {
		#region private
		enum Messages 
		{
			[DefaultMessage("For FormField file variables, please use Types of HttpPostedFileBase (in System.Web.Abstractions), string, byte[], or Stream.")]
			InvalidFormFieldFileType
		}
		#endregion

		#region construction
		public ControllerHandler(){ }
		#endregion

		#region public
		/// <summary>
		/// Creates the controller instance and initializes
		/// its resources.
		/// </summary>
		/// <param name="controllerInfo">The IControllerInfo.</param>
		/// <param name="binding">The IBinding.</param>
		/// <param name="bistroContext">The request context.</param>
		/// <returns></returns>
		public override IController GetControllerInstance(IControllerInfo controllerInfo, IBinding binding, IContext bistroContext) 
		{
			HttpContextBase httpContext = bistroContext.HttpContext;
			HttpSessionStateBase session = bistroContext.Session;
			
			Type type = controllerInfo.MemberInfo as Type;
			IController controller = type.GetConstructor(Type.EmptyTypes).Invoke(null) as IController;
			//IController controller = Type.GetType(controllerInfo.Name).GetConstructor(Type.EmptyTypes).Invoke(null) as IController;
			controller.Initialize();
			
			// the order here matters. we may have something that's both a session field
			// and a request field. want to make sure that it gets set in a consistent fashion

			// also, keep checking for presence of values. this way, if something is
			// marked as more than one scope, and is present in one but not the other,
			// it won't get overriden with a null.
			
			// Parameters...
			string[] parameterValues = binding.ParameterValuesIn(bistroContext.Url);
			string[] parameterNames = binding.ParameterNames;
			for (int i = 0; i < parameterValues.Length; i++) {
				IResource parameter = controllerInfo.Resources.Parameter(parameterNames[i]);
				if(parameter != null)
					SetValue(controller, parameter.MemberInfo, parameterValues[i]);
			}

			// Session resources...
			IResource[] sessionResources = controllerInfo.Resources.GetBy<SessionAttribute>();
			if (session.Keys != null) {
				ArrayList sessionKeys = new ArrayList(session.Keys);
				foreach (IResource resource in sessionResources)
					if (sessionKeys.Contains(resource.Name))
						SetValue(controller, resource.MemberInfo, session[resource.Name]);
			}

			// Cookie resources...
			//TODO: both the allCookies and allFormFields collections should be computed once per request, not for every controller?
			IResource[] cookieResources = controllerInfo.Resources.GetBy<CookieReadAttribute>();
			if (httpContext.Request.Cookies != null) {
				List<string> allCookieKeys = new List<string>(httpContext.Request.Cookies.AllKeys);
				foreach (IResource resource in cookieResources)
					if (allCookieKeys.Contains(resource.Name))
						SetValue(controller, resource.MemberInfo, httpContext.Request.Cookies[resource.Name].Value);
			}
			
			// File form field resources...
			IResource[] formFieldResources = controllerInfo.Resources.GetBy<FormFieldAttribute>();
			HttpFileCollectionBase files = httpContext.Request.Files;
			if (files != null) {
				ArrayList fileFormFieldKeys = new ArrayList(files.AllKeys);
				foreach (IResource resource in formFieldResources)
					if (fileFormFieldKeys.Contains(resource.Name))
						SetFileValue(controller, resource.MemberInfo, files[resource.Name]);
			}
			
			// Other form field resources...
			if (httpContext.Request.Form != null && httpContext.Request.Form.AllKeys != null) {
				List<string> allFormFieldKeys = new List<string>(httpContext.Request.Form.AllKeys);
				foreach (IResource resource in formFieldResources)
					if (allFormFieldKeys.Contains(resource.Name))
						SetValue(controller, resource.MemberInfo, httpContext.Request.Form[resource.Name]);
			}

			// Request resources...
			IResource[] requestResouces = controllerInfo.Resources.GetBy<RequestAttribute>();
			foreach (IResource resource in requestResouces)
				if (bistroContext.Contains(resource.Name))
					SetValue(controller, resource.MemberInfo, bistroContext[resource.Name]);
			
			return controller;
		}
		
		/// <summary>
		/// Prepares the controller for a new lifecycle.
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="controllerInfo"></param>
		/// <param name="binding"></param>
		/// <param name="bistroContext"></param>
		public override void ReturnController(IController controller, IControllerInfo controllerInfo, IBinding binding, IContext bistroContext)
		{
			HttpContextBase httpContext = bistroContext.HttpContext;

			// Populate outbound values
			IResource[] cookieResouces = controllerInfo.Resources.GetBy<CookieWriteAttribute>();
			foreach (IResource resource in cookieResouces) {
				HttpCookie newCookie = httpContext.Response.Cookies[resource.Name] ?? new HttpCookie(resource.Name);
				var val = GetValue(controller, resource.MemberInfo);
				if (val != null)
					newCookie.Value = Convert.ToString(val);

				newCookie.Expires = val == null ? DateTime.MinValue : DateTime.MaxValue;
				httpContext.Response.Cookies.Add(newCookie);
			}

			IResource[] requestResources = controllerInfo.Resources.GetBy<RequestAttribute>();
			foreach(IResource resource in requestResources)
				bistroContext[resource.Name] = GetValue(controller, resource.MemberInfo);
			
			IResource[] sessionResources = controllerInfo.Resources.GetBy<SessionAttribute>();
			foreach (IResource resource in sessionResources)
				bistroContext[resource.Name] = GetValue(controller, resource.MemberInfo);
			
			// Allow controller to recycle
			controller.Recycle();

			// Set all controller resouces to null/default values.
			IResource[] allResources = controllerInfo.Resources.All;
			foreach (IResource resource in allResources) {
				if(resource.HasAttribute<IResourceAttribute>() || resource.IsParameter)
					SetValue(controller, resource.MemberInfo, null);
			}
		}
		#endregion

		#region protected
		protected void SetFileValue(object instance, MemberInfo member, HttpPostedFileBase file) 
		{
			PropertyInfo pInfo = member as PropertyInfo;
			if (pInfo == null) {
				FieldInfo fInfo = (FieldInfo)member;
				if (file == null || string.IsNullOrEmpty(file.FileName))
					fInfo.SetValue(instance, null);
				else if (fInfo.FieldType == typeof(HttpPostedFile))
					throw new WebException(StatusCode.InternalServerError, Message.GetDefault(Messages.InvalidFormFieldFileType));
				else if (fInfo.FieldType == typeof(HttpPostedFileBase))
					fInfo.SetValue(instance, file);
				else if (fInfo.FieldType == typeof(String))
					fInfo.SetValue(instance, ReadFileText(file));
				else if (fInfo.FieldType == typeof(Stream))
					fInfo.SetValue(instance, file.InputStream);
				else if (fInfo.FieldType == typeof(byte[]))
					fInfo.SetValue(instance, ReadFileBytes(file));
				else
					throw new WebException(StatusCode.InternalServerError, Message.GetDefault(Messages.InvalidFormFieldFileType));
			}
			else
				if (file == null || string.IsNullOrEmpty(file.FileName))
					pInfo.SetValue(instance, null, null);
				else if (pInfo.PropertyType == typeof(HttpPostedFile))
					throw new WebException(StatusCode.InternalServerError, Message.GetDefault(Messages.InvalidFormFieldFileType));
				else if (pInfo.PropertyType == typeof(HttpPostedFileBase))
					pInfo.SetValue(instance, file, null);
				else if (pInfo.PropertyType == typeof(String))
					pInfo.SetValue(instance, ReadFileText(file), null);
				else if (pInfo.PropertyType == typeof(Stream))
					pInfo.SetValue(instance, file.InputStream, null);
				else if (pInfo.PropertyType == typeof(byte[]))
					pInfo.SetValue(instance, ReadFileBytes(file), null);
				else
					throw new WebException(StatusCode.InternalServerError, Message.GetDefault(Messages.InvalidFormFieldFileType));
		}
				
		/// <summary>
		/// Sets the value of a PropertyInfo or FieldInfo represented by member to equal value.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="member">The member.</param>
		/// <param name="value">The value.</param>
		protected void SetValue(object instance, MemberInfo member, object value)
		{
			PropertyInfo pInfo = member as PropertyInfo;
			if (pInfo == null)
			{
			   FieldInfo fInfo = (FieldInfo)member;
				fInfo.SetValue(instance, value == null ? null : Convert.ChangeType(value, fInfo.FieldType));
			}
			else
				pInfo.SetValue(instance, value == null ? null : Convert.ChangeType(value, pInfo.PropertyType), null);
		}

		/// <summary>
		/// Gets the value of a PropertyInfo or FieldInfo represented by member.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="member">The member.</param>
		/// <returns></returns>
		protected object GetValue(object instance, MemberInfo member)
		{
			PropertyInfo pInfo = member as PropertyInfo;
			if (pInfo == null)
			{
				FieldInfo fInfo = (FieldInfo)member;
				return fInfo.GetValue(instance);
			}
			else
				return pInfo.GetValue(instance, null);
		}
		#endregion

		#region private methods
		byte[] ReadFileBytes(HttpPostedFileBase file) {
			byte[] bytes = new byte[file.ContentLength];
			using (Stream stream = file.InputStream) {
				stream.Read(bytes, 0, file.ContentLength);
			}
			return bytes;
		}
		string ReadFileText(HttpPostedFileBase file) {
			string text = System.Text.UTF8Encoding.UTF8.GetString(ReadFileBytes(file));
			return text;
		}
		#endregion
	}
}
