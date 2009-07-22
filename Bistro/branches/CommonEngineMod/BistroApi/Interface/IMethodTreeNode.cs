using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Contains the method, binding, and controller information
	/// for each node of a tree structure base on URL paths.
	/// </summary>
	public interface IMethodTreeNode 
	{
		string Name { get; }
		IMethodTreeNode Parent { get; }
		List<IMethodTreeNode> Children { get; }
		IMethod Method { get; }
		string Path { get; }
	}
}
