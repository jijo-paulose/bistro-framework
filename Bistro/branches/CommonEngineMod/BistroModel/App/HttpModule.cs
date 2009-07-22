using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
using BistroApi;
using System.Threading;

namespace BistroModel
{
	/// <summary>
	/// Interfaces with .NET framework to provide bistro functionality. This class uses the asp.net mvc url routing approach
	/// for url re-writing. The request is intercepted during the PostResolveRequestCache and saved off onto the context. 
	/// It is then re-routed to a static page with a .bistro extension to bypass the StaticFileHandler. The pipeline is then
	/// again intercepted during PostMapRequestHandler and the original request and computed handler are put back in.
	/// </summary>
	public class HttpModule : IHttpModule {
		#region private
		enum Messages
		{
			[DefaultMessage("Application has not been properly initialized")]
			NotInitialized,
			[DefaultMessage("No web session factory defined")]
			NoSessionFactory,
			[DefaultMessage("Application is being unloaded")]
			Unloading,
			[DefaultMessage("Exception processing URL ({0}) {1}\r\n\tSession: N/A. For additional information, reference {2}. ")]
			ExceptionNoSession,
			[DefaultMessage("Exception processing URL ({0}) {1}\r\n\tSession: ID={2} Activity: {3} ({4}). For additional information, reference {5}.")]
			Exception,
			[DefaultMessage("Headers are \r\n{0}")]
			Headers,
			[DefaultMessage("Extended information for trace {0}. Review attached parameters.")]
			ExtendedInfo,
			[DefaultMessage("Initializing Application")]
			Initializing,
			[DefaultMessage("Waiting for parallel initialization to complete")]
			Waiting,
			[DefaultMessage("Parallel initialization completed")]
			Initialized
		}
		HttpHandler _handler;
		ILoader _loader;
		HttpApplication _context;
		
		BRequestFilter _requestFilter;
		ILogger _logger;
		#endregion

		#region IHttpModule Members
		/// <summary>
		/// Initializes the http module
		/// </summary>
		/// <param name="context"></param>
		public virtual void Init(HttpApplication context) {
			_logger = Global.Application.CreateLogger(null);
			_logger.Report(Messages.Initializing);
			
			_context = context;
			IDispatcher dispatcher = Global.Application.CreateDispatcher();
			_loader = Global.Application.CreateLoader(dispatcher);
			_loader.Load();
			dispatcher.BuildMethods();
			_handler = new HttpHandler(dispatcher);
			_requestFilter = new BRequestFilter(Global.Application);

			_context.PostResolveRequestCache += new EventHandler(context_PostResolveRequestCache);

			_logger.Report(Messages.Initialized);
		}
		public void Dispose() {}
		#endregion
		
		#region protected
		/// <summary>
		/// Gets the HttpApplication.
		/// </summary>
		/// <value>The HttpApplication.</value>
		protected HttpApplication Context { get { return _context; } }
		#endregion

		#region private methods
		/// <summary>
		/// Handles the PostResolveRequestCache event of the context control. This point is used to 
		/// intercept the handling of a bistro url and re-route to "Rewrite.bistro" as a way to 
		/// bypass the StaticFileHandler which is automatically invoked for extension-less urls.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void context_PostResolveRequestCache(object sender, EventArgs e) {
			if (!_requestFilter.IsValidPath(HttpContext.Current.Request.Path, HttpContext.Current.Request.ApplicationPath))
				return;
			// Note: this requires .Net 2.0 SP2 or later:
			HttpContext ctx = ((HttpApplication) sender).Context;
			ctx.RemapHandler(_handler);
		}
		/// <summary>
		/// Gets the type of the application.
		/// </summary>
		/// <value>The type of the application.</value>
		Type zApplicationType { get { return typeof(object); } }
		#endregion
	}
}