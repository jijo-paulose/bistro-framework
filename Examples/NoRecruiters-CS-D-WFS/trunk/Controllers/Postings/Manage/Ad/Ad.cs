using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using NoRecruiters3;
using NoRecruiters3.Helpers;

namespace NoRecruiters.Controllers.Postings.Manage.Ad
{
    /// <summary>
    /// Displays the ad screen
    /// </summary>
    [Bind("get /posting/ad/byname/{shortName}")]
    [RenderWith(@"Templates\Posting\Ad\edit.django")]
    public class AdDisplay: PostingBase
    {
        public override void DoProcessRequest(IExecutionContext context) { }
    }

    /// <summary>
    /// Applies ad changes
    /// </summary>
    [Bind("post /posting/ad/byname/{shortName}")]
    [RenderWith(@"Templates\Posting\Ad\edit.django")]
    public class AdUpdate : PostingBase
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
            {
                SetPublishState();
            }
            else
            {
                UpdatePosting();
            }
            context.Transfer("/posting/manage");

        }

        private void SetPublishState()
        {
            if (posting == null)
                return;

            if (Convert.ToBoolean(published))
                posting.Publish();
            else
                posting.Suspend();

            return;
        }

        private void UpdatePosting()
        {
            posting.User = currentUser;
            posting.Heading = heading;

            PostingHelper.Instance.PopulatePosting(detail, tags, posting, ContentType.Job);

            posting.Save();

            if (shortName == "profile")
            {
                currentUser.Posting = posting;
                currentUser.Save();

                // automatically publish if this is the profile
                if (!(posting.Published ?? false))
                    posting.Publish();
            }
        }
    }

}
