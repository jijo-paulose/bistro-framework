using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Abstract base class for IApplication.
	/// </summary>
	public abstract class ApplicationBase : IApplication
	{
		#region IApplication Members

		public virtual ILoader CreateLoader(IDispatcher dispatcher) {
			throw new NotImplementedException();
		}

		public virtual IDispatcher CreateDispatcher() {
			throw new NotImplementedException();
		}

		public virtual ILogger CreateLogger(Type type) {
			throw new NotImplementedException();
		}

		public virtual string[] AllowedExtensions {
			get { throw new NotImplementedException(); }
		}

		public virtual string[] IgnoredDirectories {
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
