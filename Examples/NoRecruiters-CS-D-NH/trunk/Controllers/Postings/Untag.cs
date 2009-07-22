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
using System.Collections.Generic;

using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;

namespace NoRecruiters.Controllers.Actions
{
    /// <summary>
    /// Untagging aspect controller. This controller reacts to any request
    /// that has an untagging aspect to it, parsing the {tag} parameter
    /// and modifying the currentTags sesion variable to reflect the removed
    /// value. 
    /// </summary>
    [Bind("get ?/without-tag/{tag}")]
    public class Untag : AbstractController
    {
        protected string tag;

        [Session]
        protected List<string> currentTags;

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (currentTags == null)
                return;

            currentTags.Remove(tag);
        }
    }
}
