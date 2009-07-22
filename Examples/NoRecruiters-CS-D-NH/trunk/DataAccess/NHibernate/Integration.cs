using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers;
using NHibernate.Context;
using Bistro.Controllers.Security;

namespace NoRecruiters.DataAccess.NHibernate
{
    /// <summary>
    /// Provides an NHibernate session for consumption by the rest of the controller chain. 
    /// This controller is implemented as a security controller to make sure that other security
    /// controllers have access to the context.
    /// </summary>
    [Bind("?")]
    public class NHibernateSetupController : AbstractController, ISecurityController
    {
        static NHibernateSetupController()
        {
            NHibernateManager.Instance.Initialize();
        }

        [Request]
        protected NHDataContext dataContext;

        public override void DoProcessRequest(IExecutionContext context)
        {
            dataContext = new NHDataContext();
            dataContext.Setup();

            CurrentSessionContext.Bind(dataContext.CurrentSession());

            // if we've been authenticated before, we want to make sure that we reconnect the user profile
            // to the current session.
            //if (context.CurrentUser.Identity.IsAuthenticated)
            //{
            //    var merged = (UserProfile)dataContext.CurrentSession().Merge(context.CurrentUser);
                
            //    // it's a hack, but there's no other way to set the authenticated flag directly.
            //    merged.Authenticate(merged.Password);
            //    context.Authenticate(merged);
            //}
        }

        public bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions)
        {
            return true;
        }
    }

    /// <summary>
    /// Cleans up the nhibernate session after controller execution
    /// </summary>
    [Bind("?", ControllerBindType = BindType.After)]
    public class NHibernateCleanupController : AbstractController
    {
        [Request, Requires]
        protected NHDataContext dataContext;

        public override void DoProcessRequest(IExecutionContext context)
        {
            // we only want this happening on the last ''method'' of the request
            if (context.TransferRequested)
                return;

            CurrentSessionContext.Unbind(NHibernateManager.Instance.GetFactory());

            dataContext.Cleanup();
        }
    }
}
