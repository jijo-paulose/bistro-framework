using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Represents an IControllerInfo plus its associated
	/// IBinding (a controller can have many bindings, but
	/// for a particular method there is also a particular
	/// binding) which makes up a part of a Bistro method. A
	/// Bistro method can contain many of these parts, which
	/// are in an ordered list.
	/// </summary>
	public interface IMethodPart
	{
		IBinding Binding { get; }
		IControllerInfo ControllerInfo { get; }
	}
}
