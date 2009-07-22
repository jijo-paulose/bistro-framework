using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Bistro.Controllers;
using NoRecruiters.DataAccess;
using System.Collections.Generic;
using System.Web.SessionState;
using System.Text;

using Bistro.Controllers.Descriptor.Data;

using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Security;

namespace NoRecruiters.Controllers.Actions
{
    /// <summary>
    /// First tmie search controller. This controller reacts to 
    /// "first time" searches, and uses the selected content
    /// type as the default content type for subsequent searches.
    /// this removes the need for the search controller to bother
    /// with identifying and persisting appropriate content types
    /// </summary>
    [Bind("get /postings/{contentType}?{firstTime}")]
    public class FirstTimeSearch : AbstractController
    {
        protected string contentType;

        protected bool firstTime;

        [CookieField(Name = "nrDefaultContentType", Outbound = true)]
        [Request, Requires]
        protected string defaultContentType;

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (firstTime)
                defaultContentType = contentType;
        }
    }

    /// <summary>
    /// Search controller. This controller uses form fields and parameters
    /// to formulate a correct search request
    /// </summary>
    [Bind("postings/{contentType}")]
    [RenderWith("Templates/Posting/search.django")]
    public class Search : AbstractController
    {
        [FormField]
        protected string txtQuery;

        [Session, DependsOn]
        protected List<string> currentTags;

        [Request]
        protected List<Posting.tag> popularTags;
        
        [Request]
        protected IEnumerable<Posting> searchResults;
        
        [Request]
        protected string contentType;

        public override void DoProcessRequest(IExecutionContext context)
        {
            searchResults =
                PostingDataAccess.Instance.PostingSearch(
                    txtQuery,
                    GetCurrentTagsAsCDL(),
                    ContentTypeUtility.FromString(contentType));

            popularTags = GetPopularTags();
        }

        private List<Posting.tag> GetPopularTags()
        {
            List<Posting.tag> popular = new List<Posting.tag>();
            foreach (Posting.tag tag in TagDataAccess.Instance.GetRankedTags(15))
                if (currentTags == null || !currentTags.Contains(tag.SafeText))
                    popular.Add(tag);

            return popular;
        }

        /// <summary>
        /// gets all current tags as a comma-delimited list
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private string GetCurrentTagsAsCDL()
        {
            if (currentTags == null || currentTags.Count == 0)
                return null;

            StringBuilder sbl = new StringBuilder();

            foreach (string tag in currentTags)
                sbl.Append(tag).Append(',');

            sbl.Remove(sbl.Length - 1, 1);

            return sbl.ToString();
        }
    }
}