using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers;
using NoRecruiters3.Helpers;
using NoRecruiters3;

namespace NoRecruiters.Controllers.Postings.Manage
{
    /// <summary>
    /// Displays the Apply screen, for both job ads and resumes
    /// </summary>
    [Bind("get /posting/apply/{appContentType}/{shortName}")]
    public class ApplyDisplay: AbstractController
    {
        [Request]
        protected string
            appContentType,
            shortName;

        public override void DoProcessRequest(IExecutionContext context)
        {
            switch (appContentType)
            {
                case "ad":
                    context.Response.RenderWith(@"Templates\Posting\Ad\apply.django");
                    break;
                case "resume":
                    context.Response.RenderWith(@"Templates\Posting\Resume\apply.django");
                    break;
            }
        }
    }

    /// <summary>
    /// Processes the apply screen, for both job ads and resumes
    /// </summary>
    [Bind("post /posting/apply/{appContentType}/{shortName}")]
    [RenderWith(@"Templates\Posting\applied.django")]
    public class ApplyController : AbstractController
    {
        /// <summary>
        /// Posting that's being applied for/contacted, and the content type of the posting.
        /// Notice that the name is different from "contentType", as contentType holds the
        /// default content type for the user, whereas this holds the content type of the 
        /// posting being contacted.
        /// </summary>
        [Request]
        protected string
            appContentType,
            shortName;

        [FormField, Request]
        protected string comment;

        [Request, Requires]
        protected userProfile currentUser;

        public override void DoProcessRequest(IExecutionContext context)
        {
            UserHelper.Instance.ContactPerson(
                currentUser.Posting,
                PostingHelper.Instance.LoadPosting(null, shortName).User,
                comment);
        }
    }
}
