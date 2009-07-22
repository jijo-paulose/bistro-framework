using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi {
	/// <summary>
	/// A set of actions that can be taken on a DemandAttribute failure
	/// </summary>
	[Flags]
	public enum FailAction {
		/// <summary>
		/// Throw an exception
		/// </summary>
		Fail = 1,
		/// <summary>
		/// Reditect to a supplied location
		/// </summary>
		Redirect = 2
	}
}
