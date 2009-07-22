using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WorkflowServer.Foundation.Diagnostics;
using WorkflowServer.Foundation;
using WorkflowServer.Foundation.Documents.Factories;

namespace NoRecruiters3
{

    /// <summary>
    /// Summary description for ContentType
    /// </summary>
    public partial class Posting
    {
        [SeverityLevel(Severity.Error)]
        enum Errors
        {
            [DefaultMessage("{0} is not published")]
            NotPublished,
            [DefaultMessage("{0} has already been published")]
            AlreadyPublished,
            [DefaultMessage("{0} has been deleted")]
            Deleted
        }

        public const int MAX_TAG_LENGTH = 50;

        public void Publish()
        {
            if (Published ?? false)
                WSApplication.Application.Report(Errors.AlreadyPublished, LocalID);

            if (Deleted ?? false)
                WSApplication.Application.Report(Errors.Deleted, LocalID);

            Published = true;
            Save();
        }

        public void Suspend()
        {
            if (!(Published ?? false))
                WSApplication.Application.Report(Errors.NotPublished, LocalID);

            if (Deleted ?? false)
                WSApplication.Application.Report(Errors.Deleted, LocalID);

            Published = false;
            Save();
        }

        public override WSSerializable Save()
        {
            var ret = base.Save();

            if (Contents.IsModified())
                Contents.Save();

            return ret;
        }

        public void Flag(int flag, userProfile user)
        {
            UserAction userAction = (UserAction)WSApplication.Application.ActivityRoot.GetDocumentFactory("UserAction").CreateDocument();
            userAction.ActionId = flag.ToString();

            if (user != null)
                userAction.UserId = user.LocalID;

            userAction.CreatedOn = System.DateTime.Now;
            userAction.PostingId = LocalID;
            userAction.Save();
        }

        public void Apply(userProfile user, string comment)
        {
            applications_Node node = applications.AppendNode();

            node.SubmittedBy = user;
            node.SubmittedOn = DateTime.Now;

            if (ContentType.Equals(ContentType.Job))
                node.SubmittedPosting = user.Posting;

            node.Comment = comment;

            Save();
        }
    }
}