using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
using BistroApi;

namespace BistroModel {
	/// <summary>
	/// An abstract base implementation of the <see cref="IController"/> interface
	/// </summary>
	public abstract class AbstractController : ControllerBase
	{
		/// <summary>
		/// Processes the request.
		/// </summary>
		/// <param name="requestContext"></param>
		public override void ProcessRequest(HttpContextBase context, IContext requestContext) {
			DoProcessRequest(requestContext);
		}

		/// <summary>
		/// Actual implementation of controller logic
		/// </summary>
		public abstract void DoProcessRequest(IExecutionContext context);

		/// <summary>
		/// Gets a value indicating whether this <see cref="IController"/> is reusable. If reusable and Stateful,
		/// the recycle method will be called once request processing is complete.
		/// </summary>
		/// <value><c>true</c> if reusable; otherwise, <c>false</c>.</value>
		public override bool Reusable { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="IController"/> is stateful. If reusable and Stateful,
		/// the recycle method will be called once request processing is complete.
		/// </summary>
		/// <value><c>true</c> if stateful; otherwise, <c>false</c>.</value>
		public override bool Stateful { get { return true; } }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public override void Initialize() { }

		/// <summary>
		/// Recycles this instance. Recycle is called after ProcessRequest completes for stateful reusable controllers.
		/// This method is intended to put the controller in a state ready for a new request. This method may not be
		/// called from the request execution thread.
		/// </summary>
		public override void Recycle() { }

		/// <summary>
		/// Gets a global type-system identifier for this class of controllers. In most cases this is simply the Type of
		/// the controller class.
		/// </summary>
		/// <returns></returns>
		public override MemberInfo GlobalHandle { get { return GetType(); } }
	}
}
