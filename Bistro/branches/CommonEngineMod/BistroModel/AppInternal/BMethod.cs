using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BistroApi;

namespace BistroModel
{
	/// <summary>
	/// Represents a single Bistro Method call.
	/// A method call executes a chain of controllers...
	/// 
	/// Internally, this chain of controllers is made up
	/// of a BListNodeSet containing a linked list of
	/// BListNode nodes.
	/// </summary>
	internal class BMethod : IMethod {
		#region private
		enum Messages {
			[DefaultMessage("No web session factory defined")]
			NoSessionFactory,
			[DefaultMessage("\tMethod not found at: {0}.")]
			MethodNotFoundAt,
			[DefaultMessage("\tFound method at: {0} with a length of {1}.")]
			FoundMethodAt,
			[DefaultMessage("\t\t{0:000} {1} is in chain of execution at binding {2}.")]
			ControllerInChain,
			[DefaultMessage("Redirecting to {0} based on requirement(s) for \r\n{1}")]
			SecurityRedirect,
			[DefaultMessage("Processing request {0}")]
			ProcessingRequest
		}
		enum Exceptions {
			[DefaultMessage("The following permission(s) were not satisfied\r\n{0}")]
			AccessDenied
		}
		List<BListNode> _listNodes;
		List<BListNode> _unchainedListNodes;
		IUrl _url;
		ILogger _logger;
		#endregion

		#region construction
		public BMethod(List<BListNode> listNodes, List<BListNode> unchainedListNodes, IUrl url) 
		{
			_listNodes = listNodes;
			_unchainedListNodes = unchainedListNodes;
			_url = url;
			_logger = Global.Application.CreateLogger(null);
		}
		#endregion
		
		#region public
		/// <summary>
		/// Executes this method. (The entire chain
		/// of controllers is executed by within the
		/// current IContext.)
		/// </summary>
		/// <param name="bistroContext"></param>
		public void Execute(IContext bistroContext) 
		{
			if (_listNodes == null)
				throw new NoMethodFoundException(Message.GetDefault(Messages.MethodNotFoundAt, bistroContext.Url.Name));

			bool securityCheckComplete = false;
			bool securityCheckFailed = false;
			var failedPermissions = new Dictionary<string, KeyValuePair<FailAction, string>>();

			//execute each controller...
			foreach (BListNode ln in _listNodes) {
				IControllerHandler controllerHandler = ln.ControllerInfo.Handler;
				IController controller = controllerHandler.GetControllerInstance(ln.ControllerInfo, ln.Binding, bistroContext);

				try {
					// all security controllers are guaranteed to be at the top of the list, in proper order.
					// we need to run through all of them, because an inner controller may override the decision 
					// of an outer controller. therefore, the final decision is deferred until all security checks
					// have passed
					if (!securityCheckComplete) {
						ISecurityController securityController = controller as ISecurityController;
						if (securityController == null)
							securityCheckComplete = true;
						else {
							// this needs to run prior to the check
							controller.ProcessRequest(bistroContext.HttpContext, bistroContext);

							// we only care about the actual return value of the last controller, as intermediate
							// results can be overriden by subsequent controllers. discard intermediate return values.
							securityCheckFailed = !securityController.HasAccess(bistroContext, failedPermissions);
						}
					}

					// we have to do the check a second time, because the securityCheckComplete flag
					// gets set inside the top if. doing this in an else up top would skip the first
					// non-security controller
					if (securityCheckComplete) {
						if (securityCheckFailed || failedPermissions.Count != 0) {
							StringBuilder builder = new StringBuilder();
							foreach (string perm in failedPermissions.Keys)
								builder.AppendLine(perm);

							foreach (KeyValuePair<FailAction, string> kvp in failedPermissions.Values)
								if (kvp.Key == FailAction.Redirect) {
									// currently you can only house one transfer request per context, 
									// however, that may change in the future.
									bistroContext.ClearTransferRequest();
									//bistroContext.Transfer(kvp.Value);
									// give the fail action target a chance to redirect after re-validating
									bistroContext.Transfer(
											kvp.Value +
											(kvp.Value.Contains("?") ? "&" : "?") +
											"originalRequest=" +
											HttpUtility.UrlEncode(bistroContext.Url.Name));

									//WSApplication.Application.Report(Messages.SecurityRedirect, kvp.Value, builder.ToString());
									_logger.Report(Messages.SecurityRedirect, kvp.Value, builder.ToString());
									break;
								}

							if (!bistroContext.TransferRequested)
								throw new WebException(StatusCode.Unauthorized, "Access denied");
							else
								// break out of the controller loop. we shouldn't be processing any more
								// controllers for this request, and need to get into whatever the security
								// guys requested
								break;
						}
						else {
							if (ln.ControllerInfo.DefaultTemplate != null)
								bistroContext.Response.RenderWith(ln.ControllerInfo.DefaultTemplate);
							controller.ProcessRequest(bistroContext.HttpContext, bistroContext);
						}
					}
				}
				finally {
					controllerHandler.ReturnController(controller, ln.ControllerInfo, ln.Binding, bistroContext);
				}
			}
			if (bistroContext.TransferRequested) {
				string transferRequestPoint = bistroContext.TransferTarget;
				bistroContext.ClearTransferRequest();
				bistroContext.Url = new BUrl(transferRequestPoint, HttpAction.GET);

				Execute(bistroContext);
			}
		}
		/// <summary>
		/// The IUrl that corresponds to this method's binding.
		/// </summary>
		public IUrl Url { get { return _url; } }
		/// <summary>
		/// Shows this method. (The entire chain
		/// of controllers is reported on.)
		/// </summary>
		/// <param name="bistroContext"></param>
		public void Show(IUrl url) {
			if (_listNodes == null){
				_logger.Report(Messages.MethodNotFoundAt, url.Name);
				return;
			}
			_logger.Report(Messages.FoundMethodAt, url.Name, _listNodes.Count.ToString());
			//show each controller...
			int i = 1;
			foreach (BListNode ln in _listNodes) {
				_logger.Report(Messages.ControllerInChain, i, ln.ControllerInfo.Name, ln.Binding.Name);
				i++;
			}
		}
		/// <summary>
		/// Method parts (IControllerInfos and associated IBindings)
		/// that have all dependencies resolved.
		/// </summary>
		public List<IMethodPart> LinkedMethodParts {
			get {
				List<IMethodPart> list = new List<IMethodPart>();
				if (_listNodes != null) {
					foreach (BListNode ln in _listNodes)
						list.Add(ln);
				}
				return list;
			}
		}
		/// <summary>
		/// Method parts (IControllerInfos and associated IBindings)
		/// that have unresolved dependencies.
		/// </summary>
		public List<IMethodPart> UnlinkedMethodParts {
			get {
				List<IMethodPart> list = new List<IMethodPart>();
				if (_unchainedListNodes != null) {
					foreach (BListNode ln in _unchainedListNodes)
						list.Add(ln);
				}
				return list;
			} 
		}
		#endregion
	}
}
