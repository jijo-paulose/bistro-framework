using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using BistroApi;
using System.Reflection;

namespace BistroModel
{
	/// <summary>
	/// Rendering controller for using both Velocity and Django templates
	/// </summary>
	[Bind("?", ControllerBindType = BindType.Payload)]
	public abstract class RenderingController : ControllerBase
	{
		/// <summary>
		/// Processes the request.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="requestContext"></param>
		public override void ProcessRequest(HttpContextBase context, IContext requestContext)
		{
			// if transfer is requested, no rendering should be done
			if (requestContext.TransferRequested)
				return;

			if (requestContext.Response.RenderTarget == null)
			{
				if (requestContext.Response.CurrentReturnType == BistroApi.ReturnType.Template)
					throw new ApplicationException("No template specified");

				return;
			}

			var attrs = (TemplateMappingAttribute[])GetType().GetCustomAttributes(typeof(TemplateMappingAttribute), true);
			foreach (TemplateMappingAttribute attr in attrs)
				if (requestContext.Response.RenderTarget.EndsWith(attr.Extension))
				{
					((HttpHandler)context.Handler).GetTemplateEngine(EngineType).Render(context, requestContext);
					return;
				}
		}

		protected abstract Type EngineType { get; }

		/// <summary>
		/// Initializes this instance. This method is called before system-manipulated fields have been populated.
		/// </summary>
		public override void Initialize() { }

		/// <summary>
		/// Recycles this instance. Recycle is called after ProcessRequest completes for stateful reusable controllers.
		/// This method is intended to put the controller in a state ready for a new request. This method may not be
		/// called from the request execution thread.
		/// </summary>
		public override void Recycle() { }

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
		public override bool Stateful { get { return false; } }

		public override MemberInfo GlobalHandle { get { return GetType(); } }

	}
}
