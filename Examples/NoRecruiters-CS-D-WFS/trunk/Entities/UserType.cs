using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WorkflowServer.Foundation.Documents;
using WorkflowServer.Foundation.Documents.Queries;
using WorkflowServer.Foundation;

namespace NoRecruiters3
{

    /// <summary>
    /// Summary description for ContentType
    /// </summary>
    public partial class UserType
    {

        internal static UserType Retrieve(string localId)
        {
            DocumentFactory contentTypeFactory = WSApplication.Application.ActivityRoot.GetDocumentFactory("UserType");
            QueryParams p = contentTypeFactory.NewQueryParams();
            p["docID"] = localId;

            return (UserType)contentTypeFactory.SelectSingleDocument(p);
        }

        public static UserType Company { get { return Retrieve("0"); } }

        public static UserType Person { get { return Retrieve("1"); } }

        public static UserType Recruiter { get { return Retrieve("2"); } }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return ((Document)obj).LocalID == LocalID;
        }

        public override int GetHashCode()
        {
            return LocalID.GetHashCode();
        }

        public override string ToString()
        {
            return LocalID;
        }

        public ContentType ContentType
        {
            get
            {
                if (UserType.Company.Equals(this))
                    return ContentType.Resume;
                else if (UserType.Person.Equals(this))
                    return ContentType.Job;

                return null;
            }
        }
    }
}