using System.Collections.Generic;
using System.Security.Principal;
using System;
namespace NoRecruiters
{

    /// <summary>
    /// User profile object. This class is used for both authentication/authorization/access 
    /// and for storing profile information.
    /// </summary>
    public class UserProfile : IPrincipal, IIdentity
    {
        public virtual IList<role> roles { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual Posting Posting { get; set; }

        public virtual UserType UserType { get; set; }

        public virtual string Id { get; set; }

        /// <summary>
        /// Represents a single role granted to the owning user.
        /// </summary>
        public class role
        {

            public role() { }

            public role(string name)
            {
                this.Name = name;
            }

            public virtual string Name
            { get; set; }

            public override bool Equals(object obj)
            {
                var target = obj as role;

                if (target == null)
                    return false;

                return target.Name.Equals(Name);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public virtual string AuthenticationType
        {
            get { return "FORMS"; }
        }

        public virtual bool IsAuthenticated
        {
            get;
            private set;
        }

        public virtual string Name { get { return UserName; } }

        public virtual IIdentity Identity { get { return this; } }

        public virtual bool IsInRole(string role)
        {
            return roles.Contains(new role(role));
        }

        public virtual bool Authenticate(string password)
        {
            IsAuthenticated = password == this.Password;

            return IsAuthenticated;
        }

        public override bool Equals(object obj)
        {
            var target = obj as UserProfile;

            if (target == null)
                return false;

            return target.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
