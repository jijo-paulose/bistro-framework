using System;
using Bistro.Controllers.Security;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers;
using NoRecruiters;

namespace NoRecruiters.Controllers.Security
{
    [Bind("get /posting/manage")]
    [Bind("get /posting/ad/applicants/byId/{adId}")]
    [Deny("*", OnFailure = FailAction.Redirect, Target = "/auth/signin")]
    [Allow("company")]
    public class CompanyFunctionAccessControl : SecurityController { }

    
    [Bind("/posting")]
    [Deny("?", OnFailure = FailAction.Redirect, Target = "/auth/signin")]
    public class GeneralFunctionAccessControl: SecurityController
    {
        [Request]
        protected UserProfile currentUser;

        public override void DoProcessRequest(IExecutionContext context)
        {
            base.DoProcessRequest(context);

            if (context.CurrentUser.Identity.IsAuthenticated)
                currentUser = (UserProfile)context.CurrentUser;
        }
    }
}
