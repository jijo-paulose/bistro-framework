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
using System.Reflection;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;
using System.Text.RegularExpressions;
using System.IO;
using Bistro.Http;

namespace NoRecruiters3.Controllers
{
    /// <summary>
    /// Controller responsible for resetting the default content type settings
    /// </summary>
    [Bind("/default/{preferenceReset}")]
    public class ClearPreferences : AbstractController
    {
        [Request]
        protected bool preferenceReset = true;

        /// <summary>
        /// This controller needs to be able to set the cookie, but also associate
        /// to the existing value on the request. We have this set to "Requires" because 
        /// we want to make sure that this value has already been populated elsewhere
        /// </summary>
        [CookieField(Name = "nrDefaultContentType", Outbound = true)]
        [Request, Requires]
        protected string defaultContentType;

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (preferenceReset)
                defaultContentType = null;
        }
    }

    /// <summary>
    /// Home page controller. 
    /// </summary>
    [Bind("/default")]
    [RenderWith("Templates/home.django")]
    public class Home : AbstractController
    {
        [Request, DependsOn]
        protected bool preferenceReset;

        [Request, Requires]
        protected string defaultContentType;

        public override void DoProcessRequest(IExecutionContext context)
        {
            ContentType def = ContentTypeHelper.Instance.Parse(defaultContentType);

            // if preferences have been reset elsewhere, we need to stay on this page.
            if (def == null || preferenceReset)
                return;

            // otherwise, we have a default content type, so we redirect to the search page for it
            context.Transfer("/postings/" + (def.Equals(ContentType.Resume) ? "resume" : "ad"));
        }

    }

    /// <summary>
    /// Controller for retrieving static content
    /// </summary>
    [Bind("/static/{contentId}")]
    public class Static : AbstractController
    {
        public string contentId;

        static Regex invalidNames = new Regex(@"\\|/|\.\.|:", RegexOptions.Compiled);

        public override void DoProcessRequest(IExecutionContext context)
        {
            // since this gets used (eventually) as an absolute path, make sure to validate
            // for invalid character sequences
            if (invalidNames.IsMatch(contentId))
                throw new WebException(StatusCode.NotFound, String.Format("'{0}' could not be found", contentId));

            context.Response.RenderWith("static/" + contentId + ".django");
        }
    }
}
