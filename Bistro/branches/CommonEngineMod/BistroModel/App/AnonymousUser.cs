using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace BistroModel
{
    /// <summary>
    /// The anonymous user. This class is threadsafe.
    /// </summary>
    public class AnonymousUser: IPrincipal, IIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousUser"/> class.
        /// </summary>
        private AnonymousUser() { }

        /// <summary>
        /// Gets an instance of an anonymous user.
        /// </summary>
        /// <value>The instance.</value>
        public static AnonymousUser Instance { get; private set; }

        /// <summary>
        /// Initializes the <see cref="AnonymousUser"/> class.
        /// </summary>
        static AnonymousUser()
        {
            Instance = new AnonymousUser();
        }

        #region IPrincipal Members

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.
        /// </returns>
        public IIdentity Identity
        {
            get { return this; }
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        public bool IsInRole(string role)
        {
            switch (role)
            {
                case "?": return true;
                default: return false;
            }
        }

        #endregion

        #region IIdentity Members

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType
        {
            get { return "FORMS"; }
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <value></value>
        /// <returns>true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name
        {
            get { return "Anonymous"; }
        }

        #endregion
    }
}
