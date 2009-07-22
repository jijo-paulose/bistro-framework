using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Abstract base class for IControllerHandler.
	/// </summary>
	public abstract class ControllerHandlerBase : IControllerHandler
	{
		#region IControllerHandler Members

		public virtual IController GetControllerInstance(IControllerInfo controllerInfo, IBinding binding, IContext bistroContext) {
			throw new NotImplementedException();
		}

		public virtual void ReturnController(IController controller, IControllerInfo controllerInfo, IBinding binding, IContext bistroContext) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
