using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers;

namespace NoRecruiters.Controllers.Postings.Manage.Resume
{
    /// <summary>
    /// Displays the preview screen for a user's resume. If the user
    /// doesn't have a resume defined, auto-transfers to the edit
    /// screen.
    /// </summary>
    [Bind("get /posting/resume/preview/byname/{shortName}")]
    [RenderWith(@"Templates\Posting\Resume\preview.django")]
    public class PreviewDisplay : PostingBase
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            if (posting == null)
                context.Transfer("get /posting/resume/byname");
        }
    }
}
