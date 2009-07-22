using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace NoRecruiters3.Helpers
{
    /// <summary>
    /// Manages content types and persiting "favorite" content type information
    /// </summary>
    public class ContentTypeHelper
    {
        private static ContentTypeHelper instance = new ContentTypeHelper();

        private const string COOKIE_NAME = "nrDefaultContentType";

        private ContentTypeHelper() { }

        public static ContentTypeHelper Instance { get { return instance; } }

        /// <summary>
        /// Parses the content type.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public ContentType Parse(string contentType)
        {
            if (contentType == null)
                return null;

            if (contentType.Equals("ad"))
                return ContentType.Job;
            else if (contentType.Equals("resume"))
                return ContentType.Resume;
            else
                return ContentType.Retrieve(contentType);
        }

        /// <summary>
        /// Returns the string id representation of the content type id. If a null or invalid
        /// id is supplied, the return value defaults to "ad"
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public string GetSimpleFromId(string id)
        {
            if (ContentType.Resume.Equals(ContentType.Retrieve(id)))
                return "resume";

            return "ad";
        }

        /// <summary>
        /// Gets the simple id of the supplied content type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetSimpleName(ContentType type)
        {
            return ContentType.Resume.Equals(type) ? "resume" : "ad";
        }
    }
}