using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Bistro.Controllers;
using NoRecruiters3.Helpers;
using WAPNoRecruiters3.Controllers;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;

namespace NoRecruiters3.Controllers.Postings
{
    [Bind("get /ad/{shortName}")]
    [Bind("get /resume/{shortName}")]
    [RenderWith("Templates/Posting/view.django")]
    public class View : AbstractController
    {
        [Request, DependsOn]
        protected string contentType;

        [Request]
        protected Posting posting;

        [Request, Requires]
        protected string defaultContentType;

        protected string shortName;

        public override void DoProcessRequest(IExecutionContext context)
        {
            posting = PostingHelper.Instance.LoadPosting(null, shortName);
            contentType = defaultContentType;
        }
    }
}
