using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BistroApi;
using System.Web.SessionState;
using System.Security;

namespace BistroModel
{
	/// <summary>
	/// Bistro Http Handler. This class is the transition point from ASP.NET into
	/// Bistro controllers.
	/// </summary>
	public class HttpHandler: IHttpHandler, IRequiresSessionState {
		#region private
		enum Messages
		{
			[DefaultMessage("No web session factory defined")]
			NoSessionFactory,
			[DefaultMessage("{0} could not be associated with a bind point")]
			ControllerNotFound,
			[DefaultMessage("Redirecting to {0} based on requirement(s) for \r\n{1}")]
			SecurityRedirect,
			[DefaultMessage("No method found at {0}.")]
			NoMethodFoundAt
		}
		enum Exceptions
		{
			[DefaultMessage("The following permission(s) were not satisfied\r\n{0}")]
			AccessDenied
		}
		/// <summary>
		/// The key used to store the bistro session on the asp.net session
		/// </summary>
		const string SESSION_ID = "bistroSession";
		IDispatcher _dispatcher;
		ILogger _logger;
		Dictionary<Type, TemplateEngine> _templateEngines;
		#endregion

		#region construction
		public HttpHandler() {
			_templateEngines = new Dictionary<Type, TemplateEngine>();
			_logger = Global.Application.CreateLogger(null); 
		}
		public HttpHandler(IDispatcher dispatcher) : this()
		{
			_dispatcher = dispatcher;
		}
		#endregion

		#region public
		/// <summary>
		/// The name of the session factory
		/// </summary>
		public const string WEB_SESSION_FACTORY = "web-session";
		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
		public bool IsReusable{ get { return true; } }
		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		public void ProcessRequest(HttpContext context) {
			string requestPoint;
			if (context.Request.RawUrl == "/default.aspx?")
				requestPoint = "/";
			else
				requestPoint = context.Request.RawUrl.Substring(context.Request.ApplicationPath.Length);
			
			IContext bistroContext = CreateRequestContext(context, requestPoint);
			context.User = bistroContext.CurrentUser;
			try {
				_dispatcher.Dispatch(bistroContext);
			}
			catch (NoMethodFoundException) {
				_logger.Report(Messages.NoMethodFoundAt, bistroContext.Url.Name);
				Signal404(context.Response);
			}
			catch (WebException webEx) {
				context.Response.Clear();
				context.Response.StatusCode = Convert.ToInt16(webEx.Code);
				if (!String.IsNullOrEmpty(webEx.Message))
					context.Response.Write(webEx.Message);

				if (webEx.InnerException != null && webEx.Code == StatusCode.InternalServerError)
					context.Response.Write("\r\n\r\n" + webEx.ToString());
			}
			//catch (Exception) {
				//todo?
			//}
		}
		#endregion

		#region internal
		internal TemplateEngine GetTemplateEngine(Type engineType) {
			TemplateEngine result;
			if (!_templateEngines.TryGetValue(engineType, out result)) {
				if (typeof(TemplateEngine).IsAssignableFrom(engineType) && engineType.GetConstructor(new Type[] { GetType() }) != null) {
					result = (TemplateEngine)Activator.CreateInstance(engineType, this);
					_templateEngines.Add(engineType, result);
				}
				else
					throw new Exception(engineType.FullName + " is not a valid type for rendering engine");
			}
			return result;
		}
		#endregion

		#region protected
		/// <summary>
		/// Creates an <see cref="T:Bistro.Controller.IContext"/>
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		protected virtual IContext CreateRequestContext(HttpContext context, string requestPoint)
		{
			var contextWrapper = new HttpContextWrapper(context);
			return new BistroContext(contextWrapper, new BUrl(context.Request.HttpMethod + requestPoint));
		}
		/// <summary>
		/// Retrieves the bistro session from the asp.net session
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The bistro session</returns>
		protected virtual ISession GetSession(HttpContext context) {
			ISession session = context.Session[SESSION_ID] as ISession;

			if (session == null) {
				//DocumentFactory f = WSApplication.GetDocumentFactory(WEB_SESSION_FACTORY);
				//if (f == null || !f.Type.IsSubclassOf(typeof(Session)))
				//	return null;// throw new WDFException(Messages.NoSessionFactory);
				//session = (Session)f.CreateDocument();
				//context.Session[SESSION_ID] = session;
				//throw new ApplicationException("fix this!");
				session = new BSession();
				context.Session[SESSION_ID] = session;
			}

			return session;
		}
		#endregion

		#region private methods
		/// <summary>
		/// Returns a 404 code
		/// </summary>
		/// <param name="httpResponse">The HTTP response.</param>
		void Signal404(HttpResponse httpResponse)
		{
			httpResponse.Clear();
			httpResponse.StatusCode = 404;
			httpResponse.Write(String.Format("404 - requested resource could not be found"));
			httpResponse.End();
		}
		#endregion
	}
}
