using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Abstract base class used for all Bistro Resource Attributes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
	public abstract class AbstractResourceAttribute : Attribute, IResourceAttribute
	{
		string _name;
		/// <summary>
		/// Overrides the actual resource name for the use of a
		/// particular attribute.
		/// </summary>
		public virtual string Name { get { return _name; } set { _name = value; } }
	}
}
