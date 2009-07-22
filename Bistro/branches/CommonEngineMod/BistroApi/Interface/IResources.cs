using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi {
	/// <summary>
	/// Used for obtaining information about the set of resources
	/// and parameters within a IControllerInfo.
	/// </summary>
	public interface IResources 
	{
		IResource[] GetBy<TResourceAttribute>() where TResourceAttribute : IResourceAttribute;
		bool HasAny<TResourceAttribute>() where TResourceAttribute : IResourceAttribute;
		bool HasNo<TResourceAttribute>() where TResourceAttribute : IResourceAttribute;
		bool Contains<TResourceAttribute>(IResource resource) where TResourceAttribute : IResourceAttribute;
		IResource[] All { get; }
		IResource[] Parameters { get; }
		IResource Parameter(string name);
	}
}
