/****************************************************************************
 * 
 *  Bistro Framework Copyright © 2003-2009 Hill30 Inc
 *
 *  This file is part of Bistro Framework.
 *
 *  Bistro Framework is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Bistro Framework is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Bistro Framework.  If not, see <http://www.gnu.org/licenses/>.
 *  
 ***************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Web;
using System.Collections;
using Bistro.Controllers.Descriptor;
using Bistro.Configuration.Logging;

namespace Bistro.Controllers
{
	/// <summary>
	/// Manages the state of a controller pre and post processing.
	/// </summary>
	public class ControllerHandler : Bistro.Controllers.IControllerHandler
	{
		enum Messages 
		{
			[DefaultMessage("For FormField file variables, please use Types of HttpPostedFile, string, byte[], or Stream.")]
			InvalidFormFieldFileType,
            [DefaultMessage("Unable to set {0}.{1} to \"{2}\" because \"{3}\"")]
            UnableToSetValue
		}

		/// <summary>
		/// The controller managed by this handler. The specific bind-point is determined at the point of invocation.
		/// </summary>
		ControllerDescriptor descriptor;

		/// <summary>
		/// A list of all fields modified by the system
		/// </summary>
		List<MemberInfo> manipulatedFields = new List<MemberInfo>();

        /// <summary>
        /// Empty object array for no-parameter method signatures
        /// </summary>
        private object[] EmptyParams = new object[] { };

        /// <summary>
        /// Constructor for our controller
        /// </summary>
        protected ConstructorInfo controllerConstructor;

        /// <summary>
        /// Our logger
        /// </summary>
        private ILogger logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerHandler"/> class.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		protected internal ControllerHandler(ControllerDescriptor descriptor, ILogger logger)
		{
			this.descriptor = descriptor;
            this.logger = logger;

            controllerConstructor = ((Type)descriptor.ControllerType).GetConstructor(Type.EmptyTypes);

			foreach (ControllerDescriptor.BindPointDescriptor bindPoint in descriptor.Targets)
				manipulatedFields.AddRange(bindPoint.ParameterFields.Values);
			manipulatedFields.AddRange(descriptor.FormFields.Values);
			manipulatedFields.AddRange(descriptor.RequestFields.Values);
			manipulatedFields.AddRange(descriptor.SessionFields.Values);
			foreach (ControllerDescriptor.CookieFieldDescriptor cookieField in descriptor.CookieFields.Values)
				manipulatedFields.Add(cookieField.Field);
		}

		/// <summary>
		/// Gets the controller instance.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		/// <param name="requestContext">The request context.</param>
		/// <returns></returns>
		public virtual IController GetControllerInstance(ControllerInvocationInfo info, HttpContextBase context, IContext requestContext)
		{
			IController instance = controllerConstructor.Invoke(EmptyParams) as IController;
			instance.Initialize();

			// the order here matters. we may have something that's both a session field
			// and a request field. want to make sure that it gets set in a consistent fashion

			// also, keep checking for presence of values. this way, if something is
			// marked as more than one scope, and is present in one but not the other,
			// it won't get overriden with a null.

			foreach (string field in info.Parameters.Keys)
				// not all path parameters may be present on the controller
				if (info.BindPoint.ParameterFields.ContainsKey(field))
					SetValue(
						instance, 
						info.BindPoint.ParameterFields[field], 
						info.Parameters[field]);

			// asking for a non-existant session value will simply return null
			ArrayList allSessionFields = new ArrayList(context.Session.Keys);
			foreach (string sessionField in descriptor.SessionFields.Keys)
				if (allSessionFields.Contains(sessionField))
					SetValue(instance, descriptor.SessionFields[sessionField], context.Session[sessionField]);

			//TODO: both the allCookies and allFormFields collections should be computed once per request, not for every controller
			List<string> allCookies = new List<string>(context.Request.Cookies.AllKeys);
			foreach (string cookie in descriptor.CookieFields.Keys)
				if (allCookies.Contains(cookie))
					SetValue(instance, descriptor.CookieFields[cookie].Field, context.Request.Cookies[cookie].Value);
			
			HttpFileCollectionBase files = context.Request.Files;
			foreach (string file in files.AllKeys)
				if (descriptor.FormFields.ContainsKey(file))
					SetFileValue(instance, descriptor.FormFields[file], files[file]);
						
			List<string> allFormFields = new List<string>(context.Request.Form.AllKeys);
			foreach (string formField in descriptor.FormFields.Keys)
				if (allFormFields.Contains(formField))
					SetValue(instance, descriptor.FormFields[formField], context.Request.Form[formField]);

			foreach (string requestField in descriptor.RequestFields.Keys)
				if (requestContext.Contains(requestField))
					SetValue(instance, descriptor.RequestFields[requestField], requestContext[requestField]);

			return instance;
		}

		/// <summary>
		/// Prepares the controller for a new lifecycle.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The http context for the current request.</param>
		/// <param name="requestContext"></param>
		public virtual void ReturnController(IController controller, HttpContextBase context, IContext requestContext)
		{
			// populate outbound values
			foreach (string cookie in descriptor.CookieFields.Keys)
                if (descriptor.CookieFields[cookie].Outbound)
                {
                    HttpCookie newCookie = context.Response.Cookies[cookie] ?? new HttpCookie(cookie);
                    var val = GetValue(controller, descriptor.CookieFields[cookie].Field);
                    if (val != null)
                        newCookie.Value = Convert.ToString(val);

                    newCookie.Expires = val == null ? DateTime.MinValue : DateTime.MaxValue;
                    context.Response.Cookies.Add(newCookie);
                }

			foreach (string requestField in descriptor.RequestFields.Keys)
				requestContext[requestField] = GetValue(controller, descriptor.RequestFields[requestField]);

			foreach (string sessionField in descriptor.SessionFields.Keys)
				context.Session[sessionField] = GetValue(controller, descriptor.SessionFields[sessionField]);

			controller.Recycle();

			foreach (MemberInfo info in manipulatedFields)
				SetValue(controller, info, null);
		}

		protected void SetFileValue(object instance, MemberInfo member, HttpPostedFileBase file) 
		{
			PropertyInfo pInfo = member as PropertyInfo;
			if (pInfo == null) {
				FieldInfo fInfo = (FieldInfo)member;
				if (file == null || string.IsNullOrEmpty(file.FileName))
					fInfo.SetValue(instance, null);
				else if (fInfo.FieldType == typeof(HttpPostedFile))
					fInfo.SetValue(instance, file);
				else if (fInfo.FieldType == typeof(String))
					fInfo.SetValue(instance, ReadFileText(file));
				else if (fInfo.FieldType == typeof(Stream))
					fInfo.SetValue(instance, file.InputStream);
				else if (fInfo.FieldType == typeof(byte[]))
					fInfo.SetValue(instance, ReadFileBytes(file));
				else
					throw new ApplicationException(Messages.InvalidFormFieldFileType.ToString());
			}
			else
				if (file == null || string.IsNullOrEmpty(file.FileName))
					pInfo.SetValue(instance, null, null);
				else if (pInfo.PropertyType == typeof(HttpPostedFile))
					pInfo.SetValue(instance, file, null);
				else if (pInfo.PropertyType == typeof(String))
					pInfo.SetValue(instance, ReadFileText(file), null);
				else if (pInfo.PropertyType == typeof(Stream))
					pInfo.SetValue(instance, file.InputStream, null);
				else if (pInfo.PropertyType == typeof(byte[]))
					pInfo.SetValue(instance, ReadFileBytes(file), null);
				else
					throw new ApplicationException(Messages.InvalidFormFieldFileType.ToString());
		}
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
				
		/// <summary>
		/// Sets the value of a PropertyInfo or FieldInfo represented by member to equal value.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="member">The member.</param>
		/// <param name="value">The value.</param>
		protected void SetValue(object instance, MemberInfo member, object value)
		{
            // potentially valid - bind parameter with no corresponding field
            if (member == null)
                return;

            try
            {
                PropertyInfo pInfo = member as PropertyInfo;
                if (pInfo == null)
                {
                    FieldInfo fInfo = (FieldInfo)member;
                    fInfo.SetValue(instance, value == null ? null : Coerce(value, fInfo.FieldType));
                }
                else
                {
                    if (pInfo.CanWrite)
                        pInfo.SetValue(instance, value == null ? null : Coerce(value, pInfo.PropertyType), null);
                }
            }
            catch (Exception ex)
            {
                // we should never crash here. handle it and move on.
                logger.Report(Messages.UnableToSetValue, member.DeclaringType.Name, member.Name, Convert.ToString(value), ex.Message);
            }
		}

        /// <summary>
        /// Coerces the value to the specified type. If the types are assignable, nothing is done. 
        /// Otherwise, an attempt is made to invoke Convert.ChangeType().
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private object Coerce(object value, Type type)
        {
            if (type.IsAssignableFrom(value.GetType()))
                return value;

            return Convert.ChangeType(value, type);
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
            {
                if (!pInfo.CanRead)
                    return null;
                else
                    return pInfo.GetValue(instance, null);
            }
		}
	}
}
