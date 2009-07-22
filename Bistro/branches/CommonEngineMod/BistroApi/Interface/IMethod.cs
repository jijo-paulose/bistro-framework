using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Represents a single Bistro Method call.
	/// A method call executes a chain of controllers...
	/// </summary>
	public interface IMethod 
	{
		/// <summary>
		/// Executes this method. (The entire chain
		/// of controllers is executed within the
		/// current IContext.)
		/// </summary>
		/// <param name="bistroContext"></param>
		void Execute(IContext bistroContext);
		/// <summary>
		/// The IUrl that corresponds to this method's binding.
		/// </summary>
		IUrl Url { get; }
		/// <summary>
		/// Shows this method. (The entire chain
		/// of controllers is reported on.)
		/// </summary>
		/// <param name="bistroContext"></param>
		void Show(IUrl url);
		/// <summary>
		/// Method parts (IControllerInfos and associated IBindings)
		/// that have all dependencies resolved.
		/// </summary>
		List<IMethodPart> LinkedMethodParts { get; }
		/// <summary>
		/// Method parts (IControllerInfos and associated IBindings)
		/// that have unresolved dependencies.
		/// </summary>
		List<IMethodPart> UnlinkedMethodParts { get; }
	}
}
