using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Represents a controller resource.
	/// </summary>
	public interface IResource 
	{
		string Name { get; }
		string DataType { get; }
		string GetAlias<TResourceAttribute>() where TResourceAttribute : IResourceAttribute;
		bool HasAttribute<TResourceAttribute>() where TResourceAttribute : IResourceAttribute;
		bool IsParameter { get; }
		MemberInfo MemberInfo { get; set; }
	}
}
