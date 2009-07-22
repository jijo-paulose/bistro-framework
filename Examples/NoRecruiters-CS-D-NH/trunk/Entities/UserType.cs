using System;
using System.Data;
using System.Configuration;

namespace NoRecruiters
{
    /// <summary>
    /// The user type.
    /// </summary>
    public enum UserType
    {
        Company,
        Person,
        Recruiter
    }

    /// <summary>
    /// Utility class for manipulating UserType enums
    /// </summary>
    public static class UserTypeUtility
    {
        /// <summary>
        /// Maps the user type to the corresponding content type.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public static ContentType AsContentType(UserType userType)
        {
            switch (userType)
            {
                case UserType.Company: return ContentType.Resume;
                default: return ContentType.Ad;
            }
        }

        /// <summary>
        /// Returns the string representation of the user type
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public static string AsString(UserType userType)
        {
            return Enum.GetName(typeof(UserType), userType).ToLower();
        }
    }
}