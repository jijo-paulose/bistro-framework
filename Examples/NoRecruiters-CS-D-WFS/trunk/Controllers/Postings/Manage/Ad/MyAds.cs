using Bistro.Controllers;
using NoRecruiters3.Helpers;
using System.Web;
using Bistro;
using WAPNoRecruiters3.Controllers;
using Bistro.Controllers.Descriptor.Data;
using System.Collections.Generic;
using WorkflowServer.Foundation.Documents;
using WorkflowServer.Foundation.Documents.Queries;
using Bistro.Controllers.Descriptor;
using WorkflowServer.Foundation;
using NoRecruiters3;


namespace NoRecruiters.Controllers.Postings.Manage.Ad
{
    /// <summary>
    /// Displays published an unpublished postings
    /// </summary>
    [Bind("get /posting/manage")]
    [RenderWith(@"Templates\Posting\Ad\Manage\myAds.django")]
    public class Manage : AbstractController
    {
        [Request]
        protected DocumentNodeCollection
            unpublished,
            published;

        [Request, Requires]
        protected userProfile currentUser;

        public override void DoProcessRequest(IExecutionContext context)
        {
            LoadData(true, currentUser.LocalID);
        }

        private void LoadData(bool reset, string userId)
        {
            unpublished = PostingHelper.Instance.UserPostingSearch(userId, reset, false);
            published = PostingHelper.Instance.UserPostingSearch(userId, reset, true);
        }
    }
}