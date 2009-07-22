using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BistroApi 
{
	/// <summary>
	/// Abstract base class for SecurityControllers (IController & ISecurityController.
	/// </summary>
	public abstract class SecurityControllerBase : ControllerBase, ISecurityController
	{
		#region ISecurityController Members

		public virtual bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
