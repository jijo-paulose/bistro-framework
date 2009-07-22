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

using WorkflowServer.Foundation.Documents;

namespace NoRecruiters3
{
    public partial class web_session: Session
    {
        public web_session(DocumentFactory factory) : base(factory) { }
    }
}
