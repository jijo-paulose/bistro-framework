using Bistro.Controllers;
using System.Web;
using NoRecruiters3.Helpers;
using WorkflowServer.Foundation.Documents;
using Bistro;
using System;
using WorkflowServer.Foundation.Documents.Queries;
using WAPNoRecruiters3.Controllers;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;
using WorkflowServer.Foundation;

namespace NoRecruiters.Controllers.Auth
{
    /// <summary>
    /// Signout controller.
    /// </summary>
    [Bind("get /auth/signout")]
    [RenderWith(@"Templates\Profile\signout.django")]
    public class SignOut : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            // The runtime will react to the null by setting the Anonymous 
            // profile as the current user. the other option is the Abandon
            // method on the context.
            context.Authenticate(null);
        }
    }
}