using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WorkflowServer.Foundation;
using WorkflowServer.Foundation.Documents;
using WorkflowServer.Foundation.Documents.Queries;

namespace NoRecruiters3.Helpers
{
    /// <summary>
    /// Summary description for UserHelper
    /// </summary>
    public class UserHelper
    {
        private static UserHelper instance = new UserHelper();

        private UserHelper() { }

        public static UserHelper Instance { get { return instance; } }

        public void ContactPerson(Posting companyProfile, userProfile contactUser, string contactText)
        {
            ContactRequest request = (ContactRequest)WSApplication.Application.ActivityRoot.GetDocumentFactory("ContactRequest").CreateDocument();

            request.CompanyName = companyProfile.Heading;
            //request.CompanyUrl = UrlRewriter.Instance.GetContentUrl(companyProfile.ContentType, companyProfile.ShortName);
            request.ContactText = contactText;
            request.ToAddress = contactUser.Email;

            request.Send("/ContactNotification");
        }

        public userProfile LoadUser(string name)
        {
            DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("userProfile");
            QueryParams p = factory.NewQueryParams();
            p["name"] = name;

            return (userProfile)factory.SelectSingleDocument(p);
        }

        public userProfile CreateUser()
        {
            return (userProfile)WSApplication.Application.ActivityRoot.GetDocumentFactory("userProfile").CreateDocument();
        }
    }
}