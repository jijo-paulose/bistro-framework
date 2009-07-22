using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Abstract base class for IDispatcher.
	/// </summary>
	public abstract class DispatcherBase : IDispatcher
	{
		#region IDispatcher Members

		public virtual void Dispatch(IContext bistroContext) {
			throw new NotImplementedException();
		}

		public virtual IBinding Register(IBinding binding) {
			throw new NotImplementedException();
		}

		public virtual void Register(IControllerInfo controllerInfo) {
			throw new NotImplementedException();
		}

		public virtual void BuildMethods() {
			throw new NotImplementedException();
		}

		public virtual IMethod GetMethodAt(IUrl url) {
			throw new NotImplementedException();
		}

		public virtual IMethodTreeNode GetRootMethodTreeNode() {
			throw new NotImplementedException();
		}

		#endregion
	}
}
