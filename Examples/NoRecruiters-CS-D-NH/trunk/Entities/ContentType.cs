using System;
using System.Data;
using System.Configuration;

namespace NoRecruiters
{
    /// <summary>
    /// Valid content types.
    /// </summary>
    public enum ContentType
    {
        Ad,
        Resume
    }

    /// <summary>
    /// Utility class for manipulating content type information
    /// </summary>
    public static class ContentTypeUtility
    {
        /// <summary>
        /// Gets the content type's string representation
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public static string AsString(ContentType contentType)
        {
            return Enum.GetName(typeof(ContentType), contentType).ToLower();
        }

        /// <summary>
        /// Gets a content type based on the string representation. If contentType is not a valid ContentType, an exception is thrown.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public static ContentType FromString(string contentType)
        {
            return (ContentType)Enum.Parse(typeof(ContentType), contentType, true);
        }

        /// <summary>
        /// Retrieves a matching user type for this content type
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public static UserType AsUserType(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Resume: return UserType.Company;
                case ContentType.Ad: return UserType.Person;
                default: return UserType.Recruiter;
            }
        }
    }
}