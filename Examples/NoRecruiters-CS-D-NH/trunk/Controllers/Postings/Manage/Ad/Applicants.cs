using Bistro.Controllers;
using NoRecruiters.DataAccess;
using System.Web;
using Bistro;

using Bistro.Controllers.Descriptor.Data;
using System.Collections.Generic;
using System.Text;
using System;
using Bistro.Controllers.Descriptor;
using NoRecruiters;

namespace NoRecruiters.Controllers.Postings.Manage.Ad
{
    /// <summary>
    /// Displays applicants for a particular ad, or all applicants if no ad specified.
    /// Access to this controller is set by the data access aspect controller, but the
    /// posting it provides isn't used here.
    /// </summary>
    [Bind("get /posting/ad/applicants/byId/{adId}")]
    [RenderWith(@"Templates\Posting\Ad\Manage\applicants.django")]
    public class ViewAllApplicants : AbstractController
    {
        [Request]
        protected string adId;

        [Request, Requires]
        protected UserProfile currentUser;

        [Request]
        protected IEnumerable<Posting> postings;

        public override void DoProcessRequest(IExecutionContext context)
        {
            postings = PostingDataAccess.Instance.ApplicantSearch(String.IsNullOrEmpty(adId) ? null : adId, currentUser.Id);
        }
    }
}