using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BistroApi;

namespace BistroModel 
{
	/// <summary>
	/// Contains meta data about a controller in a
	/// convenient form.
	/// </summary>
	internal class BControllerInfo : IControllerInfo 
	{
		#region private fields
		bool _isSecurity;
		Dictionary<string, BBindingProperty> _bindingProperties = new Dictionary<string, BBindingProperty>();
		string _name;
		IResources _resources;
		MemberInfo _memberInfo;
		List<IBinding> _bindings = new List<IBinding>();
		IControllerHandler _controllerHandler;
		string _defaultTemplate;
		#endregion

		#region construction
		public BControllerInfo(MemberInfo memberInfo, IDispatcher dispatcher) 
		{
			if (memberInfo == null)
				throw new ArgumentNullException("memberInfo");
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			Type type = memberInfo as Type;
			if (type == null)
				throw new ApplicationException("This method should not be called for non-class controllers.");

			_memberInfo = memberInfo;
			LoadBindings(memberInfo, dispatcher);
			LoadMe();
			dispatcher.Register(this);
		}
		public BControllerInfo(string name, IResources resources, bool isSecurity, IControllerHandler controllerHandler) 
		{
			_name = name;
			_isSecurity = isSecurity;
			_resources = resources == null ? new BResources() : resources;
			_controllerHandler = controllerHandler;
		}
		#endregion

		#region public
		public string Name { get { return _name; } }
		public IResources Resources { get { return _resources; } }
		public bool IsSecurity { get { return _isSecurity; } }
		public int GetPriority(IBinding binding) {
			if (binding == null)
				throw new ArgumentNullException("binding");
			return GetBindingProperty(binding).Priority;
		}
		public void SetPriority(IBinding binding, int priority) {
			if (binding == null)
				throw new ArgumentNullException("binding");
			if (priority != BBindingProperty.DefaultPriority)
				GetBindingProperty(binding).Priority = priority;
		}
		public BindType GetBindType(IBinding binding) {
			if (binding == null)
				throw new ArgumentNullException("binding");
			return GetBindingProperty(binding).BindType;
		}
		public void SetBindType(IBinding binding, BindType bindType) {
			if (binding == null)
				throw new ArgumentNullException("binding");
			if (bindType != BBindingProperty.DefaultBindType)
				GetBindingProperty(binding).BindType = bindType;
		}
		public bool Has(IResource br) 
		{
			if (_resources.HasAny<ProvidesAttribute>() && _resources.Contains<ProvidesAttribute>(br))
				return true;
			return false;
		}
		/// <summary>
		/// Todo: return alternative controller handlers referenced by the
		/// HandlerAttribute.
		/// 
		/// For now, just returning the BistroModel.ControllerHandler.
		/// </summary>
		public IControllerHandler Handler { get { return _controllerHandler; } }
		public MemberInfo MemberInfo { get { return _memberInfo; } }
		public IEnumerable<IBinding> Bindings { get { return _bindings; } }
		public string DefaultTemplate { get { return _defaultTemplate; } }
		public override string ToString() {
			return string.Format("  {0} isSecurity:{1}\n    resources\n      {2}", Name, IsSecurity, _resources.ToString());
		}
		#endregion

		#region private
		BBindingProperty GetBindingProperty(IBinding binding) {
			BBindingProperty bbp = null;
			if (!_bindingProperties.TryGetValue(binding.BaseBinding.Name, out bbp)) {
				bbp = new BBindingProperty();
				_bindingProperties.Add(binding.BaseBinding.Name, bbp);
			}
			return bbp;
		}
		void LoadBindings(MemberInfo mi, IDispatcher dispatcher) {
			BindAttribute[] bindAttributes = mi.GetCustomAttributes(typeof(BindAttribute), false) as BindAttribute[];
			if (bindAttributes.Length > 0)
				foreach (BindAttribute ba in bindAttributes) {
					if(string.IsNullOrEmpty(ba.Target)){
						//no target, so default to controller name...
						var tp = mi as Type; 
						string name = (tp == null) ? mi.Name : tp.FullName;
						ba.Target = name.Replace('.', '/');
					}
					//IBinding[] bindings = new BBinding(ba);
					IBinding[] bindings = BBinding.Create(ba);
					for (int i = 0; i < bindings.Length;i++ ) {
						IBinding ib = bindings[i];
						ib = dispatcher.Register(ib);
						BBindingProperty bbp = this.GetBindingProperty(ib);
						bbp.BindType = ba.ControllerBindType;
						bbp.Priority = ba.Priority;
						ib.AddControllerInfo(this);
						_bindings.Add(ib);
					}
				}
			//else
			//	; //default binding? (documentation implies that there is a default binding, but it doesn't work that way now...)
		}
		void LoadMe() {
			//_memberInfo;
			Type type = _memberInfo as Type;
			_name = type.FullName;

			//IsSecurity?
			if (typeof(ISecurityController).IsAssignableFrom(type))
				_isSecurity = true;

			// load the renderwith attribute
			BReflection.IterateAttributes<RenderWithAttribute>(type, false, (attrib) => { _defaultTemplate = attrib.Template; }, null);
			
			//load resources...
			_resources = new BResources(this);

			//todo: get non-default controller handler based on HandlerAttribute...
			_controllerHandler = new ControllerHandler();
		}
		#endregion
	}
}
