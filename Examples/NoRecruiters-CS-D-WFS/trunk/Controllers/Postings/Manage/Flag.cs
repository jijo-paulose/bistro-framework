using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using NoRecruiters3;
using NoRecruiters3.Helpers;
using Bistro.Controllers;

namespace NoRecruiters.Controllers.Postings.Manage
{
    /// <summary>
    /// Flag-a-post controller
    /// </summary>
    [Bind("/posting/flag/{contentType}/{flagType}/{shortName}")]
    [RenderWith(@"Templates\Posting\flag.django")]
    public class Flag : AbstractController
    {
        protected string 
            flagType,
            contentType,
            shortName;

        [Request, Requires]
        protected userProfile currentUser;

        public override void DoProcessRequest(IExecutionContext context)
        {
            Posting posting = PostingHelper.Instance.LoadPosting(null, shortName);

            posting.Flag(int.Parse(flagType), (userProfile)context.CurrentUser);
        }
    }
}
