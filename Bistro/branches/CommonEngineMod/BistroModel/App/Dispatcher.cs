using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Dispatches HTTP requests to appropriate methods
	/// based on registered controllers.
	/// 
	/// Provides interface for registering controllers,
	/// building methods, and obtaining a method tree.
	/// 
	/// Holds all registered binding and controller infos.
	/// Dispatches http calls to proper controller chain.
	/// Builds chain on specific call or if chain is out
	/// of date. (Chain is out of date if register has been 
	/// called since last build of chain.)
	/// </summary>
	public class Dispatcher : DispatcherBase {
		#region private fields
		IEngine _engine;
		#endregion

		#region contruction
		public Dispatcher(){
			_engine = new BEngine();
		}
		#endregion

		#region public
		public override void Dispatch(IContext bistroContext) {
			IMethod method = _engine.Method(bistroContext.Url);
			method.Show(bistroContext.Url);
			method.Execute(bistroContext);
		}
		public override IBinding Register(IBinding binding) { return _engine.Add(binding); }
		public override void Register(IControllerInfo controllerInfo) { _engine.Add(controllerInfo); }
		public override void BuildMethods() { _engine.Build(); }
		public override IMethod GetMethodAt(IUrl url) {
			IMethod method = _engine.Method(url);
			method.Show(url);
			return method;
		}
		/// <summary>
		/// Returns a tree representation of Bindings + methods, etc.
		/// </summary>
		/// <returns></returns>
		public override IMethodTreeNode GetRootMethodTreeNode() { return _engine.GetRootMethodTreeNode(); }
		#endregion
	}
}
