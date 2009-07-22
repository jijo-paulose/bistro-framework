using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security;
using System.Web;
using System.Reflection;
using BistroApi;

namespace BistroModel {
	/// <summary>
	/// A base class for controllers that enforce security requirements
	/// </summary>
	public class SecurityController : SecurityControllerBase {
		/// <summary>
		/// List of demanded permissions
		/// </summary>
		protected SecurityAttribute[] requirements = null;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public override void Initialize() {
			requirements = GetType().GetCustomAttributes(typeof(SecurityAttribute), true) as SecurityAttribute[];

			// we want the allows to always be at the bottom of the list
			Array.Sort(requirements, (x, y) => (-1) * x.GetType().Name.CompareTo(y.GetType().Name));
		}

		/// <summary>
		/// Processes the request.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="requestContext"></param>
		public override void ProcessRequest(HttpContextBase context, IContext requestContext) {
			DoProcessRequest(requestContext);
		}

		/// <summary>
		/// Optional implementation of controller logic
		/// </summary>
		public virtual void DoProcessRequest(IExecutionContext context) { }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IController"/> is reusable. If reusable and Stateful,
		/// the recycle method will be called once request processing is complete.
		/// </summary>
		/// <value><c>true</c> if reusable; otherwise, <c>false</c>.</value>
		public override bool Reusable { get { return true; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IController"/> is stateful. If reusable and Stateful,
		/// the recycle method will be called once request processing is complete.
		/// </summary>
		/// <value><c>true</c> if stateful; otherwise, <c>false</c>.</value>
		public override bool Stateful { get { return false; } }

		/// <summary>
		/// Recycles this instance. Recycle is called after ProcessRequest completes for stateful reusable controllers.
		/// This method is intended to put the controller in a state ready for a new request. This method may not be
		/// called from the request execution thread.
		/// </summary>
		public override void Recycle() { }

		/// <summary>
		/// Determines whether the currently logged in user (or the anonymous user) has access
		/// to the bind point or points that this controller binds to
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="failAction">A set of actions to take based on FailAction. This will only be populated is the return value is false</param>
		/// <returns>
		/// 	<c>true</c> if access should be granted; otherwise, <c>false</c>.
		/// </returns>
		public override bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions) {
			bool passed = false;

			foreach (SecurityAttribute attrib in requirements) {
				bool hasAccess = attrib.HasAccess(context.CurrentUser);
				passed = passed || hasAccess;

				if (hasAccess) {
					if (attrib.OverridePrior) {
						// this will catch an allow attribute overriding a deny by explicit role
						if (failedPermissions.ContainsKey(attrib.Role))
							failedPermissions.Remove(attrib.Role);
						// if we failed on the authed wild-card, but this allows anonymous
						// to yank the failure. scenario is deny all (require authed) across 
						// entire app, but allow anonymous to access sign-in page
						else if (failedPermissions.ContainsKey(SecurityAttribute.AUTHENTICATED))
							failedPermissions.Remove(SecurityAttribute.AUTHENTICATED);
					}

					continue;
				}

				if (!attrib.HardFail)
					continue;

				failedPermissions.Add(attrib.Role, new KeyValuePair<FailAction, string>(attrib.OnFailure, attrib.Target));
			}

			return passed;
		}

		/// <summary>
		/// Gets a global type-system identifier for this class of controllers. In most cases this is simply the Type of
		/// the controller class.
		/// </summary>
		/// <returns></returns>
		public override MemberInfo GlobalHandle { get { return GetType(); } }
	}
}
