using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Represents a URL.
	/// Allows wildcard characters for use in Bindings.
	/// </summary>
	public interface IUrl 
	{
		string Name { get; }
		string Head { get; }
		string[] Parts { get; }
		string[] SubParts { get; }
		IUrl SubUrl { get; }
		string this[int index] { get; }
		int Length { get; }
		HttpAction HttpAction { get; }
		bool IsWild{get;}
		IQueryStringItem[] QueryStringItems { get; }
	}
}
