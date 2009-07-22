using System;
using System.Collections.Generic;
using System.Text;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Marks the controller as requiring the marked value to be present on the context.
	/// If at the time of controller dispatch no controller is found that explicitly 
	/// provides this value, an exception is thrown.
	/// </summary>
	public class RequiresAttribute : AbstractResourceAttribute
	{
	}
}
