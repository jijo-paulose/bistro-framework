using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BistroApi
{
	/// <summary>
	/// Dispatches HTTP requests to appropriate methods
	/// based on registered controllers.
	/// 
	/// Provides interface for registering controllers,
	/// building methods, and obtaining a method tree.
	/// </summary>
	public interface IDispatcher 
	{
		void Dispatch(IContext bistroContext);
		IBinding Register(IBinding binding);
		void Register(IControllerInfo controllerInfo);
		void BuildMethods();
		IMethod GetMethodAt(IUrl url);
		/// <summary>
		/// Returns a tree structure of Bindings with methods, etc.
		/// </summary>
		/// <returns></returns>
		IMethodTreeNode GetRootMethodTreeNode();
	}
}
