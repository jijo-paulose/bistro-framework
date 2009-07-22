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
using NoRecruiters;
using NoRecruiters.DataAccess;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;

namespace NoRecruiters.Controllers
{
    /// <summary>
    /// A simple filter to let templates translate content type ids into simple format
    /// </summary>
    class ContentTypeFilter : NDjango.Interfaces.ISimpleFilter
    {
        public object Perform(object contentType)
        {
            if (contentType is string)
            {
                var strContentType = (string)contentType;
                if (String.IsNullOrEmpty(strContentType))
                    return String.Empty;
                else
                    return strContentType;
            }

            return ContentTypeUtility.AsString((ContentType)contentType);
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
            ContentType lastDefault;

            if (String.IsNullOrEmpty(defaultContentType))
            {
                lastDefault = ContentType.Resume;
                defaultContentType = ContentTypeUtility.AsString(lastDefault);
            }
            else
                lastDefault = ContentTypeUtility.FromString(defaultContentType);

            userType = UserTypeUtility.AsString(
                        ContentTypeUtility.AsUserType(lastDefault));


            root = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
        }
    }
}