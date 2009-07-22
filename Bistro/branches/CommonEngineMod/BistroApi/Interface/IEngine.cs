using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Interface for a class which takes in and retains 
	/// binding and controller information and handles
	/// building of methods. Allows methods to be returned
	/// based on given URLs. Also provides a URL tree 
	/// structure containing method, binding, and controller 
	/// information at each node.
	/// </summary>
	public interface IEngine 
	{
		/// <summary>
		/// Add binding information to the engine.
		/// If a binding with the same name already
		/// exists in the engine. The existing binding
		/// is returned.
		/// </summary>
		/// <param name="binding"></param>
		/// <returns>The binding added or the already existing binding present in the engine.</returns>
		IBinding Add(IBinding binding);
		
		/// <summary>
		/// Adds controller information to the engine.
		/// If an attempt is made to add the same
		/// controller information more than once, an
		/// exception is thrown.
		/// </summary>
		/// <param name="controllerInfo"></param>
		void Add(IControllerInfo controllerInfo);

		/// <summary>
		/// Clears the engine of all current binding and
		/// controller information.
		/// </summary>
		void Clear();

		/// <summary>
		/// Build the tree with current set of controller infos.
		/// </summary>
		void Build();
		
		/// <summary>
		/// Return bistro method from engine based on given URL.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		IMethod Method(IUrl url);
		
		/// <summary>
		/// Returns a tree representation of Bindings + controller and resource info.
		/// </summary>
		/// <returns></returns>
		IMethodTreeNode GetRootMethodTreeNode();
	}
}
