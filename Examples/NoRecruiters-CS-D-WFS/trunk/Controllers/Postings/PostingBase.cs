using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor.Data;
using NoRecruiters3.Helpers;
using NoRecruiters3;

namespace NoRecruiters.Controllers.Postings
{
    /// <summary>
    /// Base requirements for controllers interacting with discrete postings.
    /// This class effectivly requires that the session be authenticated by
    /// having a hard dependency on a current user. It also delegates the 
    /// responsibility of pre-loading the posting to process to a controller
    /// up the execution chain
    /// </summary>
    public abstract class PostingBase : AbstractController
    {
        protected string shortName;

        [Request, Requires]
        protected userProfile currentUser;

        [Request, Requires]
        protected Posting posting;
    }
}
