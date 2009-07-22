using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Abstract base class for ILoader.
	/// </summary>
	public abstract class LoaderBase : ILoader
	{
		#region ILoader Members

		public virtual void Load() {
			throw new NotImplementedException();
		}

		#endregion
	}
}
