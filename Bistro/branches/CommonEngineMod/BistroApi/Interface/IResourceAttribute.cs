using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Common interface which must be implemented by all 
	/// resource attributes.
	/// </summary>
	public interface IResourceAttribute 
	{
		/// <summary>
		/// Gets the name used to associate this resource
		/// with some outside source or target such as
		/// a cookie or form field, etc. Defaults to the
		/// actual name of the resource (A resource is 
		/// field or property on a controller class
		/// that is marked with some resource attribute.)
		/// </summary>
		/// <value>The name.</value>
		string Name { get; }
	}
}
