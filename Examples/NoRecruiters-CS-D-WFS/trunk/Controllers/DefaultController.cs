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
using NoRecruiters3;
using NoRecruiters3.Helpers;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;

namespace WAPNoRecruiters3.Controllers
{
    /// <summary>
    /// A simple filter to let templates translate content type ids into simple format
    /// </summary>
    class ContentTypeFilter : NDjango.Interfaces.ISimpleFilter
    {
        public object Perform(object contentTypeId)
        {
            return ContentTypeHelper.Instance.GetSimpleFromId(contentTypeId.ToString());
        }
    }


    /// <summary>
    /// Global functionality. This controller prepopulates the request context
    /// with values that are consumed by most controllers and templates
    /// </summary>
    [Bind("?", Priority = 1)]
    public class DefaultController : AbstractController
    {
        static DefaultController()
        {
            NDjango.Template.Manager.RegisterFilter("ascontenttype", new ContentTypeFilter());
        }

        [Request]
        protected string userType = null;

        [Request]
        protected string root = null;

        [CookieField(Name = "nrDefaultContentType")]
        [Request]
        protected string defaultContentType;

        public override void DoProcessRequest(IExecutionContext context)
        {
            ContentType lastDefault = ContentTypeHelper.Instance.Parse(defaultContentType);

            if (lastDefault != null)
            {
                if (UserType.Company.ID == lastDefault.UserType.ID)
                    userType = "company";
                else if (UserType.Person.ID == lastDefault.UserType.ID)
                    userType = "person";
                else if (UserType.Recruiter.ID == lastDefault.UserType.ID)
                    userType = "recruiter";
            }
            else
                defaultContentType = "resume";

            root = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
        }
    }
}