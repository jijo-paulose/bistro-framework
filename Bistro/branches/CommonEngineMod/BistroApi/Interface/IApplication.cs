using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi
{
	/// <summary>
	/// Application Stub.
	/// Minimal interface for reporting, creating a loader,
	/// and creating a dispatcher.
	/// </summary>
	public interface IApplication 
	{
		ILoader CreateLoader(IDispatcher dispatcher);
		IDispatcher CreateDispatcher();
		ILogger CreateLogger(Type type);
		string[] AllowedExtensions { get; }
		string[] IgnoredDirectories { get; }
	}
}
