using Bistro.Controllers;
using NoRecruiters.DataAccess;
using System.Web;
using Bistro;
using Bistro.Controllers.Descriptor.Data;
using System.Collections.Generic;
using Bistro.Controllers.Descriptor;
using NoRecruiters;
using System;

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
        protected IEnumerable<Posting>
            unpublished,
            published;

        [Request, Requires]
        protected UserProfile currentUser;

        public override void DoProcessRequest(IExecutionContext context)
        {
            unpublished = PostingDataAccess.Instance.UserPostingSearch(currentUser.Id, false);
            published = PostingDataAccess.Instance.UserPostingSearch(currentUser.Id, true);
        }
    }
}