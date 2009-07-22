using System;
using System.Collections.Generic;
using System.Text;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Marks a field as linked with a value on the Request.Form collection
	/// </summary>
	public class SessionAttribute : AbstractResourceAttribute, IResourceScopeAttribute
	{
	}
}
