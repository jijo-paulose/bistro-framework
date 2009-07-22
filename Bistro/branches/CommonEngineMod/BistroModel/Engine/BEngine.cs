using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;
namespace BistroModel
{
	/// <summary>
	/// Bistro Engine.
	/// 
	/// Takes in and retains binding and controller 
	/// information and handles building of methods. Allows
	/// methods to be returned based on given URLs. Also 
	/// provides a URL tree structure containing method, 
	/// binding, and controller information at each node.
	/// 
	/// Load it with controller information (with related 
	/// resources info, etc.) 
	/// 
	/// For the runtime, hand it an incoming url request and 
	/// get back the a Bistro Method to execute.
	/// 
	/// For the designer get back a tree of methods (bindings
	/// with corresponding controller and resource info).
	/// 
	/// Builds methods on specific call or if methods are out
	/// of date. (Methods are out of date if any bindings or
	/// controller infos have been added since last Build call.
	/// </summary>
	internal class BEngine : IEngine
	{
		#region private fields
		Dictionary<string, IBinding> _bindings;
		Dictionary<string, IControllerInfo> _controllers;
		bool _modified;
		BTreeNode _rootNode;
		object _lock = new object(); //todo: May not need lock!!!
		#endregion

		#region construction
		public BEngine() {
			Init();
		}
		#endregion

		#region public
		/// <summary>
		/// Add binding information to the engine.
		/// If a binding with the same name already
		/// exists in the engine. The existing binding
		/// is returned.
		/// </summary>
		/// <param name="binding"></param>
		/// <returns>The binding added or the already existing binding present in the engine.</returns>
		public IBinding Add(IBinding binding) {
			lock (_lock) {
				_modified = true;
				IBinding b = null;
				if (!_bindings.TryGetValue(binding.Name, out b)) {
					b = binding;
					_bindings.Add(b.Name, b);
				}
				return b;
			}
		}
		
		/// <summary>
		/// Adds controller information to the engine.
		/// If an attempt is made to add the same
		/// controller information more than once, an
		/// exception is thrown.
		/// </summary>
		/// <param name="controllerInfo"></param>
		public void Add(IControllerInfo controllerInfo) {
			lock (_lock) {
				_modified = true;
				if (_controllers.ContainsKey(controllerInfo.Name))
					throw new ApplicationException(string.Format("Controller {0} was already registered.", controllerInfo.Name));

				_controllers.Add(controllerInfo.Name, controllerInfo);
			}
		}

		/// <summary>
		/// Clears the engine of all current binding and
		/// controller information.
		/// </summary>
		public void Clear() {
			lock (_lock) {
				Init();
			}
		}

		/// <summary>
		/// Build the tree with current set of controller infos.
		/// </summary>
		public void Build() {
			BuildMethods();
		}
		
		/// <summary>
		/// Return bistro method from engine based on given URL.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public IMethod Method(IUrl url) {
			if (_modified)
				BuildMethods();
			IMethod method = _rootNode.GetMethod(url);
			if (method == null)
				method = new BMethod(null, null, null);
			return method;
		}
		
		/// <summary>
		/// Returns a tree representation of Bindings + methods.
		/// </summary>
		/// <returns></returns>
		public IMethodTreeNode GetRootMethodTreeNode() {
			if (_modified)
				BuildMethods();
			return _rootNode;
		}
		#endregion

		#region private
		void Init() {
			_modified = true;
			_bindings = new Dictionary<string, IBinding>();
			_controllers = new Dictionary<string, IControllerInfo>();
			_rootNode = null;
		}

		void BuildMethods() {
			lock (_lock) {
				//...
				_rootNode = new BTreeNode();
				foreach (IBinding binding in _bindings.Values) {
					_rootNode.AddBinding(binding);
				}
				_rootNode.BuildMethods();
				_modified = false;
			}
		}
		#endregion
	}
}
