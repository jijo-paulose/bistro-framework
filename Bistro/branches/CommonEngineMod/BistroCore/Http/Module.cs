/****************************************************************************
 * 
 *  Bistro Framework Copyright © 2003-2009 Hill30 Inc
 *
 *  This file is part of Bistro Framework.
 *
 *  Bistro Framework is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Bistro Framework is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Bistro Framework.  If not, see <http://www.gnu.org/licenses/>.
 *  
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
using Bistro.Controllers;
using System.Threading;
using Bistro.Configuration.Logging;
using Bistro.Controllers.Security;
using Bistro.Controllers.Descriptor;
using System.Web.SessionState;
using Bistro.Controllers.OutputHandling;
using Bistro.Controllers.Dispatch;
using Bistro.Configuration;
using System.Configuration;

namespace Bistro.Http
{
    /// <summary>
    /// Interfaces with .NET framework to provide bistro functionality. This class uses the asp.net mvc url routing approach
    /// for url re-writing. The request is intercepted during the PostResolveRequestCache and saved off onto the context.
    /// It is then re-routed to a static page with a .bistro extension to bypass the StaticFileHandler. The pipeline is then
    /// again intercepted during PostMapRequestHandler and the original request and computed handler are put back in.
    /// </summary>
    public class Module : IHttpModule, IHttpHandler, IRequiresSessionState
	{
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
            Initialized,
            [DefaultMessage("{0} could not be associated with a bind point")]
            ControllerNotFound,
            [DefaultMessage("Redirecting to {0} based on requirement(s) for \r\n{1}")]
            SecurityRedirect,
            [DefaultMessage("Processing request {0}")]
            ProcessingRequest,
            [DefaultMessage("{0} is not a valid extension, and will be skipped")]
            InvalidExtension
        }

        enum Exceptions
        {
            [DefaultMessage("The following permission(s) were not satisfied\r\n{0}"), SeverityLevel(Severity.Critical)]
            AccessDenied
        }

        /// <summary>
        /// The logger to use
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The controller manager to use
        /// </summary>
        private IControllerManager manager;

        /// <summary>
        /// The disptacher to use
        /// </summary>
        private IControllerDispatcher dispatcher;

        private Application application;

        /// <summary>
        /// mutex handle
        /// </summary>
        static object moduleLock = new object();

        /// <summary>
        /// List of directories that will be ignored
        /// </summary>
        private List<string> ignoredDirectories = null;

        /// <summary>
        /// List of extensions that will be processed
        /// </summary>
        private List<string> allowedExtensions = null;

        /// <summary>
        /// Flags whether a wild-card mapping is present in the allowed extensions list
        /// </summary>
        private bool allowAllExtensions = false;
        
		/// <summary>
		/// Initializes the http module
		/// </summary>
		/// <param name="context"></param>
		public virtual void Init(HttpApplication context) {

            context.PostResolveRequestCache += new EventHandler(context_PostResolveRequestCache);
			lock (moduleLock) {
                SectionHandler section = (ConfigurationManager.GetSection("bistro") as SectionHandler) ?? new SectionHandler();

                LoadFactories(section);
                LoadUrlRules(section);
			}
		}

        /// <summary>
        /// Loads the allowedExtensions and ignoredDirectories lists. To speed processing, if empty,
        /// these lists should be kept null. For allowedExtensions, if this list is empty, no extensions
        /// are allowed. For ignoredDirectories, if this list is empty, all directories are treated as
        /// bind points. If not empty, the path is taken as an absolute path, starting from the app root
        /// </summary>
        protected virtual void LoadUrlRules(SectionHandler section)
        {
            if (section.AllowedExtensions.Count > 0)
            {
                allowedExtensions = new List<string>(section.AllowedExtensions.Count);

                foreach (NameValueConfigurationElement elem in section.AllowedExtensions)
                {
                    string ext = elem.Value.Trim(' ', '.').ToUpper();

                    if (ext.Contains("."))
                    {
                        logger.Report(Messages.InvalidExtension, ext);
                        continue;
                    }

                    // the empty extension should be added as just that, because the virtual
                    // path tool will yield .extension for extensions, and "" for none
                    if (ext == String.Empty)
                        allowedExtensions.Add(System.String.Empty);
                    else
                        allowedExtensions.Add("." + ext);
                }

                allowAllExtensions = allowedExtensions.Contains(".*");
            }

            if (section.IgnoredDirectories.Count > 0)
            {
                ignoredDirectories = new List<string>(section.IgnoredDirectories.Count);

                foreach (NameValueConfigurationElement elem in section.IgnoredDirectories)
                    ignoredDirectories.Add(elem.Value.Replace('\\', '/').Trim(' ', '/'));
            }
        }

        /// <summary>
        /// Loads the factories.
        /// </summary>
        protected virtual void LoadFactories(SectionHandler section)
        {
            if (Application.Instance == null || !Application.Instance.Initialized)
                Application.Initialize(section);

            application = Application.Instance;
            manager = application.ManagerFactory.GetManagerInstance();
            dispatcher = application.DispatcherFactory.GetDispatcherInstance();
            logger = application.LoggerFactory.GetLogger(GetType());
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
		public void Dispose() { }

        /// <summary>
        /// Handles the PostResolveRequestCache event of the context control. This point is used to 
        /// intercept the handling of a bistro url and remap them to the bistro HTTP handler
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void context_PostResolveRequestCache(object sender, EventArgs e)
        {
            if (!IsValidPath(HttpContext.Current.Request.Path))
                return;

            // Note: this requires .Net 2.0 SP2 or later
            ((HttpApplication)sender).Context.RemapHandler(this);
        }

        /// <summary>
        /// Determines whether the path has a valid extension for processing
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the path has a valid extension; otherwise, <c>false</c>.
        /// </returns>
        private bool hasValidExtension(string path)
        {
            if (allowAllExtensions)
                return true;

            string extension = VirtualPathUtility.GetExtension(HttpContext.Current.Request.Path).ToUpper();
            if (allowedExtensions == null)
                return String.Empty.Equals(extension);
            else
                return allowedExtensions.Contains(extension);
        }

        /// <summary>
        /// Determines whether the path starts with an ignored directory
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the path is on the ignored directory list; otherwise, <c>false</c>.
        /// </returns>
        private bool isIgnoredPath(string path)
        {
            if (ignoredDirectories == null)
                return false;
            else
            {
                string appRelativePath = path.Substring(HttpContext.Current.Request.ApplicationPath.Length).TrimStart('/');
                foreach (string directory in ignoredDirectories)
                    if (appRelativePath.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Determines whether the requested url should be treated as a bistro bind-point
        /// </summary>
        /// <param name="p">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the requested url is valid for processing; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidPath(string path)
        {
            return hasValidExtension(path) && !isIgnoredPath(path);
        }

        #region IHttpHandler implementation
        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            string requestPoint =
                BindPointUtilities.Combine(context.Request.HttpMethod, context.Request.RawUrl.Substring(context.Request.ApplicationPath.Length));
            try
            {
                var contextWrapper = new HttpContextWrapper(context);
                IContext requestContext = CreateRequestContext(contextWrapper);

                context.User = requestContext.CurrentUser;

                ProcessRequestRecursive(contextWrapper, requestPoint, requestContext);
            }
            catch (WebException webEx)
            {
                context.Response.Clear();
                context.Response.StatusCode = Convert.ToInt16(webEx.Code);
                if (!String.IsNullOrEmpty(webEx.Message))
                    context.Response.Write(webEx.Message);

                if (webEx.InnerException != null && webEx.Code == StatusCode.InternalServerError)
                    context.Response.Write("\r\n\r\n" + webEx.ToString());
            }
        }

        /// <summary>
        /// Obtains and processes the chain of controllers servicing this request. If a transfer is requested
        /// during the processing of a controller, the computed chain of controllers finishes, and then a
        /// recursive call is made to process the new contrller.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="requestPoint">The request point.</param>
        /// <param name="requestContext">The request context.</param>
        private void ProcessRequestRecursive(HttpContextBase context, string requestPoint, IContext requestContext)
        {
            logger.Report(Messages.ProcessingRequest, requestPoint);
            

            ControllerInvocationInfo[] controllers = dispatcher.GetControllers(requestPoint);

            if (controllers.Length == 0)
            {
                logger.Report(Messages.ControllerNotFound, requestPoint);
                Signal404(context.Response);
                return;
            }

            bool securityCheckComplete = false;
            bool securityCheckFailed = false;
            var failedPermissions = new Dictionary<string, KeyValuePair<FailAction, string>>();

            foreach (ControllerInvocationInfo info in controllers)
            {
                IController controller = manager.GetController(info, context, requestContext);

                try
                {
                    // all security controllers are guaranteed to be at the top of the list, in proper order.
                    // we need to run through all of them, because an inner controller may override the decision 
                    // of an outer controller. therefore, the final decision is deferred until all security checks
                    // have passed
                    if (!securityCheckComplete)
                    {
                        ISecurityController securityController = controller as ISecurityController;
                        if (securityController == null)
                            securityCheckComplete = true;
                        else
                        {
                            // this needs to run prior to the check
                            controller.ProcessRequest(context, requestContext);

                            // we only care about the actual return value of the last controller, as intermediate
                            // results can be overriden by subsequent controllers. discard intermediate return values.
                            securityCheckFailed = !securityController.HasAccess(requestContext, failedPermissions);
                        }
                    }

                    // we have to do the check a second time, because the securityCheckComplete flag
                    // gets set inside the top if. doing this in an else up top would skip the first
                    // non-security controller
                    if (securityCheckComplete)
                    {
                        if (securityCheckFailed || failedPermissions.Count != 0)
                        {
                            StringBuilder builder = new StringBuilder();
                            foreach (string perm in failedPermissions.Keys)
                                builder.AppendLine(perm);

                            foreach (KeyValuePair<FailAction, string> kvp in failedPermissions.Values)
                                if (kvp.Key == FailAction.Redirect)
                                {
                                    // currently you can only house one transfer request per context, 
                                    // however, that may change in the future.
                                    requestContext.ClearTransferRequest();

                                    // give the fail action target a chance to redirect after re-validating
                                    requestContext.Transfer(
                                        kvp.Value + 
                                        (kvp.Value.Contains("?") ? "&" : "?") + 
                                        "originalRequest=" + 
                                        HttpUtility.UrlEncode(requestPoint));

                                    logger.Report(Messages.SecurityRedirect, kvp.Value, builder.ToString());
                                    break;
                                }

                            if (!requestContext.TransferRequested)
                                throw new WebException(StatusCode.Unauthorized, "Access denied");
                            else
                                // break out of the controller loop. we shouldn't be processing any more
                                // controllers for this request, and need to get into whatever the security
                                // guys requested
                                break;
                        }
                        else
                        {
                            if (info.BindPoint.Controller.DefaultTemplate != null)
                                requestContext.Response.RenderWith(info.BindPoint.Controller.DefaultTemplate);

                            controller.ProcessRequest(context, requestContext);
                        }
                    }
                }
                finally
                {
                    manager.ReturnController(controller, context, requestContext);
                }
            }

            if (requestContext.TransferRequested)
            {
                string transferRequestPoint = BindPointUtilities.VerbQualify(requestContext.TransferTarget, "get");
                requestContext.ClearTransferRequest();

                ProcessRequestRecursive(context, transferRequestPoint, requestContext);
            }
        }

        /// <summary>
        /// Creates an <see cref="T:Bistro.Controllers.IContext"/>
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual IContext CreateRequestContext(HttpContextBase context)
        {
            return new DefaultContext(context);
        }

        /// <summary>
        /// Returns a 404 code
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        private void Signal404(HttpResponseBase httpResponse)
        {
            httpResponse.Clear();
            httpResponse.StatusCode = 404;
            httpResponse.Write(String.Format("404 - requested resource could not be found"));
            httpResponse.End();
        }

        Dictionary<Type, TemplateEngine> engines = new Dictionary<Type, TemplateEngine>();
        /// <summary>
        /// Returns an instance of the template engine od the specified type associated with this particular 
        /// HttpHandler. Each instance of HttpHandler has to maintain its own list of engines
        /// </summary>
        /// <param name="engineType"></param>
        /// <returns></returns>
        internal TemplateEngine GetTemplateEngine(Type engineType)
        {
            TemplateEngine result;
            if (!engines.TryGetValue(engineType, out result))
            {
                if (typeof(TemplateEngine).IsAssignableFrom(engineType) && engineType.GetConstructor(new Type[] { GetType() }) != null)
                {
                    result = (TemplateEngine)Activator.CreateInstance(engineType, this);
                    // there is no need for synchronzation here because the dictionary of engines
                    // belongs to the handler. There will never be another request coming here before this one 
                    // is completed 
                    engines.Add(engineType, result);
                }
                else
                    throw new Exception(engineType.FullName + " is not a valid type for a rendering engine");
            }
            return result;
        }
        #endregion
    }
}