using Bistro.Controllers;
using System.Web;
using NoRecruiters3.Helpers;
using WorkflowServer.Foundation.Documents;
using Bistro;
using System;
using WorkflowServer.Foundation.Documents.Queries;
using WAPNoRecruiters3.Controllers;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;
using WorkflowServer.Foundation;
using NoRecruiters.Controllers.Auth;
using NoRecruiters3;
//using CurrentSession = NoRecruiters3.Helpers.SessionHelper.Instance.CurrentSession;


namespace NoRecruiters.Controllers.Auth
{
    /// <summary>
    /// Sign-in display
    /// </summary>
    [Bind("get /auth/signin?{originalRequest}")]
    [RenderWith(@"Templates\Profile\signin.django")]
    public class SignInDisplay : AbstractController
    {
        /// <summary>
        /// The original request made that triggered the auth request. This property is 
        /// supplied by the runtime when the sign-in page isn't directly requested, but
        /// displayed as a result of a failed security policy
        /// </summary>
        [Request]
        protected string originalRequest;

        public override void DoProcessRequest(IExecutionContext context) { }
    }

    /// <summary>
    /// Sign-in processing
    /// </summary>
    [Bind("post /auth/signin")]
    [RenderWith(@"Templates\Profile\signin.django")]
    public class SignIn : ValidationBase
    {
        [FormField, Request]
        protected string
            username,
            originalRequest;

        /// <summary>
        /// The password from the form. This field shouldn't be re-displayed on failure,
        /// hence no "Request" marker
        /// </summary>
        [FormField]
        protected string password;

        /// <summary>
        /// The default content type. We will change this value if the user that signs in
        /// has a different content type than the cookie specifies.
        /// </summary>
        [CookieField(Name = "nrDefaultContentType", Outbound = true)]
        [Request]
        protected string defaultContentType;

        public override void DoProcessRequest(IExecutionContext context)
        {
            userProfile profile = UserHelper.Instance.LoadUser(username);
            if (profile == null || !profile.Authenticate(password))
            {
                ReportError(null, "The user name or password is incorrect");
                return;
            }

            else
                // this is the key line. this sets the profile as the 
                // current user, and marks the session as authenticated
                context.Authenticate(profile);

            if (ContentTypeHelper.Instance.Parse(defaultContentType) != profile.UserType.ContentType)
                defaultContentType = ContentTypeHelper.Instance.GetSimpleName(profile.UserType.ContentType);

            // hack. we didn't create a table to store the roles, so now 
            // we're faking them through here
            if (profile.UserType.Equals(UserType.Company)) {
                var node = profile.role.AppendNode();
                node.Name = "company";
            }

            if (String.IsNullOrEmpty(originalRequest))
                context.Transfer("/default");
            else
                context.Transfer(HttpUtility.UrlDecode(originalRequest));
        }
    }
}