using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BistroApi 
{
	/// <summary>
	/// Abstract base class for IController.
	/// </summary>
	public abstract class ControllerBase : IController
	{
		#region IController Members

		public virtual void ProcessRequest(System.Web.HttpContextBase context, IContext requestContext) {
			throw new NotImplementedException();
		}

		public virtual bool Reusable {
			get { throw new NotImplementedException(); }
		}

		public virtual bool Stateful {
			get { throw new NotImplementedException(); }
		}

		public virtual void Initialize() {
			throw new NotImplementedException();
		}

		public virtual void Recycle() {
			throw new NotImplementedException();
		}

		public virtual MemberInfo GlobalHandle {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
}
