using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BistroApi;

namespace BistroModel {
	/// <summary>
	/// Application Stub.
	/// Minimal interface for reporting, creating a loader,
	/// and creating a dispatcher.
	/// </summary>
	public class Application : ApplicationBase {
		#region construction
		public Application() {}
		#endregion

		#region public
		public override ILoader CreateLoader(IDispatcher dispatcher){return new Loader(dispatcher);}
		public override IDispatcher CreateDispatcher() { return new Dispatcher(); }
		public override ILogger CreateLogger(Type type) { return new Logger(); }
		public override string[] AllowedExtensions { get { return new string[0]; } }
		public override string[] IgnoredDirectories { get { return new string[0]; } }
		#endregion
	}
}
