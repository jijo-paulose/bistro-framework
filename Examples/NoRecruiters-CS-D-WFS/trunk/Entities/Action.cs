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
    /// Summary description for Action
    /// </summary>
    public partial class Action
    {
        internal static Action Retrieve(string localId)
        {
            DocumentFactory contentTypeFactory = WSApplication.Application.ActivityRoot.GetDocumentFactory("Action");
            QueryParams p = contentTypeFactory.NewQueryParams();
            p["docID"] = localId;

            return (Action)contentTypeFactory.SelectSingleDocument(p);
        }

        public static Action FlagRecruiter { get { return Retrieve("1"); } }

        public static Action FlagWrongTag { get { return Retrieve("2"); } }

        public static Action FlagSpam { get { return Retrieve("3"); } }

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
    }
}