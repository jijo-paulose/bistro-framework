using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using NoRecruiters;
using NoRecruiters.DataAccess;
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
        protected UserProfile currentUser;

        public override void DoProcessRequest(IExecutionContext context)
        {
            Posting posting = PostingDataAccess.Instance.LoadPosting(null, shortName);

            PostingDataAccess.Instance.SaveUserAction(
                posting.Flag(int.Parse(flagType), (UserProfile)context.CurrentUser));
        }
    }
}
