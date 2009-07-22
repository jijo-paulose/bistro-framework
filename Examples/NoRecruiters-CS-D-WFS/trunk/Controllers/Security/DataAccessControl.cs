using System;
using Bistro.Controllers.Security;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers;
using NoRecruiters3;
using Bistro.Controllers.Descriptor.Data;
using System.Collections.Generic;
using NoRecruiters3.Helpers;

namespace NoRecruiters.Controllers.Security
{
    /// <summary>
    /// Data access security aspect. This security controller validates that the current user 
    /// has access to the data that is being requested. Since this validation requires retrieving
    /// the data from the database, the controller also makes it available on the context for
    /// subsequent processing. 
    /// </summary>
    [Bind("?/byname/{shortName}")]
    [Bind("?/byId/{postingId}")]
    public class DataAccessControl : AbstractController, ISecurityController
    {
        /// <summary>
        /// The current user profile. Having this dependency here ensures that general security
        /// validation has already taken place through the function access control aspect
        /// </summary>
        [Request, Requires]
        protected userProfile currentUser;

        /// <summary>
        /// The requested posting
        /// </summary>
        [Request]
        protected Posting posting;

        protected string 
            shortName,
            postingId;

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (currentUser == null)
                return;

            // There are a few special cases we should be aware of

            // We allow access to a special "profile" id, and "new" id
            // one is accessing the user's profile, the other requests
            // that a new posting be created. Both are available to all
            // authenticated parties, therefore this controller grants
            // that access.
            if (shortName == "profile")
                posting = currentUser.Posting;
            else if (shortName == "new")
            {
                posting = PostingHelper.Instance.CreatePosting();
                posting.User = currentUser;
            }
            else if (!String.IsNullOrEmpty(shortName))
                posting = PostingHelper.Instance.LoadPosting(null, shortName);
            else
                posting = PostingHelper.Instance.LoadPosting(postingId, null);
        }

        /// <summary>
        /// Determines whether the specified user has access to the requested data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="failedPermissions">The failed permissions.</param>
        /// <returns>
        /// 	<c>true</c> if the specified context has access; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAccess(IContext context, IDictionary<string, KeyValuePair<FailAction, string>> failedPermissions)
        {
            // Access is granted if no posting has been loaded, or if the loaded
            // posting is owned by the current user. We do not populate "failedPermissions"
            // as there is nothing overridable decided by this controller.
            return
                (posting == null) ||
                (posting.User.LocalID == currentUser.LocalID);
        }
    }
}
