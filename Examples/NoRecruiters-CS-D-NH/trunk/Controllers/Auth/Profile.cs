using Bistro.Controllers;
using NoRecruiters.DataAccess;
using System;

using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;

using System.Collections.Generic;

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
            if (UserDataAccess.Instance.LoadUser(username) != null)
                ReportError(null, "A user with the same name already exists. Please choose another name");

            return ec == ErrorCount;
        }

        public override void DoProcessRequest(IExecutionContext context)
        {
            if (!Validate())
                return;

            UserProfile user = new UserProfile();

            user.UserName = username;
            user.Password = password;
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            user.UserType = 
                ContentTypeUtility.AsUserType(
                    ContentTypeUtility.FromString(defaultContentType));

            if (user.UserType == UserType.Company) {
                user.roles = new List<UserProfile.role>();
                var role = new UserProfile.role();
                role.Name = "company";
                user.roles.Add(role);
            }

            UserDataAccess.Instance.SaveUser(user);
            context.Response.RenderWith(@"Templates\Profile\registered.django");
        }
    }
}