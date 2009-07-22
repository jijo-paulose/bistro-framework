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
    public partial class ContentType
    {

        internal static ContentType Retrieve(string localId)
        {
            DocumentFactory contentTypeFactory = WSApplication.Application.ActivityRoot.GetDocumentFactory("ContentType");
            QueryParams p = contentTypeFactory.NewQueryParams();
            p["docID"] = localId;

            return (ContentType)contentTypeFactory.SelectSingleDocument(p);
        }

        public static ContentType Resume { get { return Retrieve("1"); } }

        public static ContentType Job { get { return Retrieve("0"); } }


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

        public UserType UserType
        {
            get
            {
                if (ContentType.Resume.Equals(this))
                    return UserType.Company;
                else if (ContentType.Job.Equals(this))
                    return UserType.Person;

                return null;
            }
        }
    }
}