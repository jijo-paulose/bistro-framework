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
using NoRecruiters3;
//using CurrentSession = NoRecruiters3.Helpers.SessionHelper.Instance.CurrentSession;


namespace NoRecruiters.Controllers.Auth
{
    /// <summary>
    /// Profile/registration screen display
    /// </summary>
    [Bind("get /auth/register")]
    [RenderWith(@"Templates\Profile\register.django")]
    public class RegisterDisplay : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context) { }
    }

    /// <summary>
    /// Profile/registration screen capture
    /// </summary>
    [Bind("post /auth/register")]
    [RenderWith(@"Templates\Profile\register.django")]
    public class Register : ValidationBase
    {
        [FormField, Request]
        protected string 
            username,
            email,
            firstName,
            lastName,
            password;

        [Request, Requires]
        protected string defaultContentType;

        /// <summary>
        /// Validates the supplied form elements.
        /// </summary>
        /// <returns></returns>
        protected bool Validate()
        {
            var ec = ErrorCount;
            if (String.IsNullOrEmpty(username))
                ReportError("username", "Please supply a user name");
            if (String.IsNullOrEmpty(password))
                ReportError("password", "Please supply a password");
            if (UserHelper.Instance.LoadUser(username) != null)
                ReportError(null, "A user with the same name already exists. Please choose another name");

            return ec == ErrorCount;
        }

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (!Validate())
                return;

            userProfile user = UserHelper.Instance.CreateUser();

            user.Name = username;
            user.Password = password;
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            user.UserType = ContentTypeHelper.Instance.Parse(defaultContentType).UserType;

            user.Save();

            context.Response.RenderWith(@"Templates\Profile\registered.django");
        }
    }
}