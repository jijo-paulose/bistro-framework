using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// Used for obtaining information about the set of resources
	/// and parameters within a IControllerInfo.
	/// </summary>
	internal class BResources : IResources {
		#region private fields
		Dictionary<string, IResource> _allHash = new Dictionary<string, IResource>();
		List<IResource> _parameters = new List<IResource>();
		Dictionary<string, IResource> _parameterHash = new Dictionary<string, IResource>();
		Dictionary<Type, List<IResource>> _listHash = new Dictionary<Type, List<IResource>>();
		#endregion

		#region construction
		public BResources(){}
		public BResources(IControllerInfo controllerInfo)
		{
			if (controllerInfo == null)
				throw new ArgumentNullException("controllerInfo");
			if (controllerInfo.MemberInfo == null)
				throw new ArgumentException("Resources cannot be created from a controllerInfo with a null MemberInfo property.", "controllerInfo");
			
			//find all resources from controller's memberinfo, and then
			//find parameters (s/b included in controller's resource list) based on the controller's bindings...
			SetResources(controllerInfo);
			SetParameters(controllerInfo);
		}
		#endregion

		#region public
		public void Add(IResource resource)
		{
			if (resource == null)
				throw new ArgumentNullException("resource");
			
			if (_allHash.ContainsKey(resource.Name))
				throw new ArgumentException(string.Format("Attempt to add duplicate resource {0}", resource), "resource");
			
			_allHash.Add(resource.Name, resource);
			
			if (resource.HasAttribute<ProvidesAttribute>())
				GetList<ProvidesAttribute>().Add(resource);

			if (resource.HasAttribute<DependsOnAttribute>())
				GetList<DependsOnAttribute>().Add(resource);

			if (resource.HasAttribute<RequiresAttribute>())
				GetList<RequiresAttribute>().Add(resource);

			if (resource.HasAttribute<CookieReadAttribute>())
				GetList<CookieReadAttribute>().Add(resource);
			
			if (resource.HasAttribute<CookieWriteAttribute>())
				GetList<CookieWriteAttribute>().Add(resource);
			
			if (resource.HasAttribute<FormFieldAttribute>())
				GetList<FormFieldAttribute>().Add(resource);

			if (resource.HasAttribute<RequestAttribute>())
				GetList<RequestAttribute>().Add(resource);

			if (resource.HasAttribute<SessionAttribute>())
				GetList<SessionAttribute>().Add(resource);

			if (resource.IsParameter)
				AddParameter(resource);
		}
		public IResource[] GetBy<TResourceAttribute>() where TResourceAttribute : IResourceAttribute {
			return GetList<TResourceAttribute>().ToArray();
		}
		public bool HasAny<TResourceAttribute>() where TResourceAttribute : IResourceAttribute {
			return GetList<TResourceAttribute>().ToArray().Length > 0;
		}
		public bool HasNo<TResourceAttribute>() where TResourceAttribute : IResourceAttribute {
			return !HasAny<TResourceAttribute>();
		}
		public bool Contains<TResourceAttribute>(IResource resource) where TResourceAttribute : IResourceAttribute {
			return GetList<TResourceAttribute>().Contains(resource);
		}
		public IResource[] All { get { return _allHash.Values.ToArray(); } }
		public IResource[] Parameters { get { return _parameters.ToArray(); } }
		public IResource Parameter(string name) {
			IResource resource = null;
			_parameterHash.TryGetValue(name, out resource);
			return resource;
		}
		public override string ToString() {
			return string.Format(
				"provides: {0}\n      dependsOn: {1}\n      requires: {2}\n      cookieRead: {3}\n      cookieWrite: {4}\n      formFields: {5}\n      parameters: {6}", 
				Show(GetList<ProvidesAttribute>()),
				Show(GetList<DependsOnAttribute>()),
				Show(GetList<RequiresAttribute>()), 
				Show(GetList<CookieReadAttribute>()),
				Show(GetList<CookieWriteAttribute>()), 
				Show(GetList<FormFieldAttribute>()), 
				Show(_parameters));
		}
		#endregion

		#region private
		List<IResource> GetList<TResourceAttribute>() where TResourceAttribute : IResourceAttribute {
			List<IResource> list = null;
			if (_listHash.TryGetValue(typeof(TResourceAttribute), out list))
				return list;
			list = new List<IResource>();
			_listHash.Add(typeof(TResourceAttribute), list);
			return list;
		}
		string Show(List<IResource> list) {
			StringBuilder sb = new StringBuilder();
			string comma = "";
			if (list != null)
				foreach (BResource br in list) {
					sb.Append(comma + br.ToString());
					if (comma.Length == 0)
						comma = ",";
				}
			return sb.ToString();
		}
		/// <summary>
		/// Populates IResources. This method will load all available data off of the 
		/// metadata placed onto the given controller type.
		/// </summary>
		/// <param name="ci">IControllerInfo controllerInfo (should have a non-null MemberInfo property.)</param>
		void SetResources(IControllerInfo controllerInfo){
			var type = controllerInfo.MemberInfo as Type;
			if (type == null)
				throw new ApplicationException("This method should not be called for non-class controllers.");


			BReflection.IterateMembers(type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
				(member) => {
					
					//Our resource...
					BResource resource = new BResource(member);
					
					//DependsOn...
					BReflection.IterateAttributes<DependsOnAttribute>(member, true,
						(attribute) => { resource.AddAttribute<DependsOnAttribute>(attribute); }, null);

					//Requires...
					BReflection.IterateAttributes<RequiresAttribute>(member, true,
						(attribute) => { resource.AddAttribute<RequiresAttribute>(attribute); }, null);

					//CookieRead...
					BReflection.IterateAttributes<CookieReadAttribute>(member, true,
						(attribute) => { resource.AddAttribute<CookieReadAttribute>(attribute); }, null);

					//CookieWrite...
					BReflection.IterateAttributes<CookieWriteAttribute>(member, true,
						(attribute) => { resource.AddAttribute<CookieWriteAttribute>(attribute); }, null);

					//FormField...
					BReflection.IterateAttributes<FormFieldAttribute>(member, true,
						(attribute) => { resource.AddAttribute<FormFieldAttribute>(attribute); }, null);

					//Request...
					BReflection.IterateAttributes<RequestAttribute>(member, true,
						(attribute) => { resource.AddAttribute<RequestAttribute>(attribute); }, null);

					//Session...
					BReflection.IterateAttributes<SessionAttribute>(member, true,
						(attribute) => { resource.AddAttribute<SessionAttribute>(attribute); }, null);

					//Provides...
					// all fields that are not marked as required or depends-on are defaulted to "provides"
					// But, maybe a specific "Provides" attribute is used because a name alias is needed?
					// Can a resource "DependsOn" and also "Provides"?
					if ((!resource.HasAttribute<RequiresAttribute>() && !resource.HasAttribute<DependsOnAttribute>()) && (resource.HasAttribute<SessionAttribute>() || resource.HasAttribute<RequestAttribute>())) {
						ProvidesAttribute[] pArray = type.GetCustomAttributes(typeof(ProvidesAttribute), true) as ProvidesAttribute[];
						if (pArray.Length > 0)
							resource.AddAttribute<ProvidesAttribute>(pArray[0]);
						else
							resource.AddAttribute<ProvidesAttribute>(new ProvidesAttribute());
					}

					this.Add(resource);
				}
			);
		}
		void SetParameters(IControllerInfo ci){
			foreach (IBinding binding in ci.Bindings) {
				foreach (string parameterName in binding.ParameterNames) {
					IResource resource = null;
					if(_allHash.TryGetValue(parameterName, out resource)){
						AddParameter(resource);
					}
					else {
						// Try for F# (with mangled name...)
						// F# members are mangled by appending an @ followed by an address
						string mangledToken = parameterName + "@";
						IResource[] resources = _allHash.Values.ToArray();
						foreach (IResource r in resources) {
							if (r.Name.StartsWith(mangledToken)) {
								AddParameter(resource, parameterName);
								break;
							}
						}
					}
				}
			}
		}
		void AddParameter(IResource resource) {
			AddParameter(resource, resource.Name);
		}
		void AddParameter(IResource resource, string name) {
			if (!_parameterHash.ContainsKey(name)) {
				_parameterHash.Add(name, resource);
				_parameters.Add(resource);
			}
		}
		#endregion
	}
}
