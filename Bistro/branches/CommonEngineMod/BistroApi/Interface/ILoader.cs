using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Loads IControllerInfos from assemblies.
	/// Dispatcher is passed to IControllerInfo constructor
	/// to allow registration of bindings and controllers.
	/// </summary>
	public interface ILoader 
	{
		void Load();
	}
}
