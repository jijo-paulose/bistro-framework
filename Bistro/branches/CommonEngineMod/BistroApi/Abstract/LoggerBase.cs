using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Abstract base class for ILogger.
	/// </summary>
	public abstract class LoggerBase : ILogger
	{
		#region ILogger Members

		public virtual Exception Report(Exception e) {
			throw new NotImplementedException();
		}

		public virtual void Report(Enum code, Exception e, params object[] args) {
			throw new NotImplementedException();
		}

		public virtual void Report(Enum code, params object[] args) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
