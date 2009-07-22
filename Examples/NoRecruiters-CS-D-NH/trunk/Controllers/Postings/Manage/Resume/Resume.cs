using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers;
using NoRecruiters.DataAccess;
using NoRecruiters;

namespace NoRecruiters.Controllers.Postings.Manage.Resume
{
    /// <summary>
    /// Displays a resume for editing
    /// </summary>
    [Bind("get /posting/resume/byname/{shortName}")]
    [RenderWith(@"Templates\Posting\Resume\edit.django")]
    public class ResumeDisplay : PostingBase
    {
        public override void DoProcessRequest(IExecutionContext context) { }
    }

    /// <summary>
    /// Applies resume edits. This controller is used in two capacities - editing
    /// resume details, and setting publish state. Both operations are done as
    /// post requests, one supplying the "published" data, the other not.
    /// </summary>
    [Bind("post /posting/resume/byname/{shortName}")]
    [RenderWith(@"Templates\Posting\Resume\edit.django")]
    public class ResumeUpdate : PostingBase
    {
        [FormField, Request]
        protected string
            heading,
            tags,
            detail,
            published;

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (!String.IsNullOrEmpty(published))
                SetPublishState();
            else
                UpdatePosting();

            context.Transfer("/posting/resume/preview/byname/" + posting.ShortName);
        }

        private void SetPublishState()
        {
            if (posting == null)
                return;

            if (Convert.ToBoolean(published))
                posting.Publish();
            else
                posting.Suspend();

            PostingDataAccess.Instance.SavePosting(posting);
        }

        private void UpdatePosting()
        {
            posting.User = currentUser;
            posting.Heading = heading;

            PostingDataAccess.Instance.PopulatePosting(detail, tags, posting, ContentType.Ad);

            // we want to update the loaded posting regardless, but if the user doesn't have
            // a default yet, we also want to save that to the db
            bool hasPosting = currentUser.Posting == null;
            currentUser.Posting = posting;

            PostingDataAccess.Instance.SavePosting(posting);
            UserDataAccess.Instance.SaveUser(currentUser);
        }
    }
}
