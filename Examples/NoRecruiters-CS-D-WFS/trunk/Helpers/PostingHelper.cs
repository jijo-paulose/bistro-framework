using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using WorkflowServer.Foundation.Documents;
using WorkflowServer.Foundation.Documents.Queries;
using Bistro;
using WorkflowServer.Foundation;

namespace NoRecruiters3.Helpers
{
    public class TagHelper
    {
        static TagHelper instance = new TagHelper();

        private TagHelper() { }


        private IEnumerable<string> NormalizeTags(string tags)
        {
            string[] tagsArray = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> dedupedTags = new List<string>(tagsArray.Length);

            for (int i = 0; i < tagsArray.Length; i++)
            {
                string tag = tagsArray[i].Trim();
                tag = tag.Substring(0, Math.Min(Posting.MAX_TAG_LENGTH, tag.Length));

                if (dedupedTags.Contains(tag))
                    continue;

                dedupedTags.Add(tag);
            }

            return dedupedTags;
        }

        public static TagHelper Instance { get { return instance; } }

        public void LoadTags(Posting posting, string tags)
        {
            if (tags == null)
                return;

            Dictionary<string, Posting.tags_Node> reference = new Dictionary<string, Posting.tags_Node>();
            List<Posting.tags_Node> dupList = new List<Posting.tags_Node>();
            foreach (Posting.tags_Node node in posting.tags)
            {
                if (reference.ContainsKey(node.SafeText))
                {
                    dupList.Add(node);
                    continue;
                }

                reference[node.SafeText] = node;
            }

            foreach (string tag in NormalizeTags(tags))
            {
                if (reference.ContainsKey(tag))
                {
                    reference.Remove(tag);
                    continue;
                }

                Posting.tags_Node node = posting.tags.AppendNode();
                node.TagText = tag;
                node.SafeText = NormalizeTagText(tag);
            }

            // remove deleted tags
            foreach (Posting.tags_Node node in reference.Values)
                posting.tags.Remove(node);

            // remove dup tags
            foreach (Posting.tags_Node node in dupList)
                posting.tags.Remove(node);
        }

        private string NormalizeTagText(string tag)
        {
            StringBuilder ret = new StringBuilder(Convert.ToInt32(tag.Length * 1.2));
            foreach (char c in tag)
                ret.Append(StringHandler.Instance.Lookup(c));

            return ret.ToString();
        }

        public string GetTagsText(Posting posting)
        {
            StringBuilder tags = new StringBuilder();
            foreach (Posting.tags_Node node in posting.tags)
            {
                if (tags.Length > 0)
                    tags.Append(", ");

                tags.Append(node.TagText);
            }

            return tags.ToString();
        }

        public Tags.data_NodeCollection GetTags(int count)
        {
            DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("Tags");
            QueryParams p = factory.NewQueryParams();

            p["count"] = count;

            return ((Tags)factory.SelectSingleDocument(p)).data;
        }

        public void RemoveTag(string p)
        {
            string currentList = "";// Session.SearchByTag;

            if (currentList == null)
                return;

            StringBuilder newList = new StringBuilder();

            string[] tags = currentList.Split(',');

            foreach (string tag in tags)
                if (tag.Trim() != p)
                    newList.Append(",").Append(tag.Trim());

            if (newList.Length > 0)
                newList.Remove(0, 1);

            //Session.SearchByTag = newList.ToString();
        }

        public void AddTag(string p)
        {
            string currentList = "";// Session.SearchByTag;

            if (currentList != null && currentList.Trim().Equals(String.Empty))
                currentList = null;

            StringBuilder newList = new StringBuilder();

            if (currentList != null)
            {
                string[] tags = currentList.Split(',');

                foreach (string tag in tags)
                    newList.Append(",").Append(tag.Trim());
            }

            newList.Insert(0, p);

            //Session.SearchByTag = newList.ToString();
        }
    }

    /// <summary>
    /// Summary description for PostingHelper
    /// </summary>
    public class PostingHelper
    {
        const int shortTextLength = 500;
        const int shortNameLength = 100;

        static PostingHelper instance = new PostingHelper();
        static string htmlPattern = @"<(.|\n)*?>";

        public static PostingHelper Instance { get { return instance; } }

        private PostingHelper() { }


        public string StripSummaryText(string htmlSummary)
        {
            string s = htmlSummary;
            int ending = htmlSummary.LastIndexOf('>');
            int starting = htmlSummary.LastIndexOf('<');

            if (ending < starting)
                s = s.Substring(0, ending + 1);

            return Regex.Replace(s, htmlPattern, string.Empty);
        }

        public void PopulatePosting(string contents, string tags, Posting posting, ContentType contentType)
        {
            if (posting.IsNew)
            {
                posting.ContentType = contentType;
                posting.CreatedOn = DateTime.Now;
//                posting.User = Session.UserProfile;
            }

            if (posting.Contents == null)
                posting.Contents = (Contents)WSApplication.Application.ActivityRoot.GetDocumentFactory("Contents").CreateDocument();

            posting.Contents.ContentsText = contents;

            if (contents != null)
            {
                string shortText = StripSummaryText(contents);
                posting.ShortText = shortText.Substring(0, Math.Min(shortTextLength, shortText.Length));
            }
            else
                posting.ShortText = null;

            if (posting.Heading != null)
            {
                string salt = posting.LocalID.Substring(posting.LocalID.Length - 6);
                string shortName = posting.Heading.Replace(' ', '_');
                posting.ShortName = shortName.Substring(0, Math.Min(shortNameLength - salt.Length, shortName.Length)) + salt;
            }

            posting.LastModifiedOn = DateTime.Now;

            TagHelper.Instance.LoadTags(posting, tags);
        }

        private DocumentNodeCollection EmptySearch(ContentType contentType)
        {
            DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("Postings");
            QueryParams p = factory.NewQueryParams();
            p["content_type"] = contentType.LocalID;

            Postings postings = factory.SelectSingleDocument(p) as Postings;
            return postings.data;
        }

        public DocumentNodeCollection PostingSearch(string query, string tags, ContentType contentType)
        {
            if (query == null && tags == null)
                return EmptySearch(contentType);

            DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("SearchPostings");
            QueryParams p = factory.NewQueryParams();
            p["content_type"] = contentType.LocalID;
            p["precision"] = 2;

            if (tags != null && !tags.Equals(string.Empty))
                p["tag_list"] = tags;
            if (query != null && !query.Equals(string.Empty))
                p["query"] = "\"" + query + "\"";

            SearchPostings postings = factory.SelectSingleDocument(p) as SearchPostings;
            return postings.data;
        }

        public DocumentNodeCollection UserPostingSearch(string userId, bool reset, bool published)
        {
            DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("UserPostings");
            QueryParams parms = factory.NewQueryParams();
            parms["user_id"] = userId;
            parms["published"] = published;

            return (factory.SelectSingleDocument(reset, parms) as UserPostings).data;
        }

        public DocumentNodeCollection ApplicantSearch(string postingId, string userId)
        {
            if (!String.IsNullOrEmpty(postingId))
            {
                DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("Applicants");
                QueryParams p = factory.NewQueryParams();
                //p["posting_id"] = posting.LocalID;
                p["posting_id"] = postingId;
                Applicants postings = factory.SelectSingleDocument(p) as Applicants;
                return postings.data;
            }
            else
            {
                DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("AllApplicants");
                QueryParams p = factory.NewQueryParams();
                p["user_id"] = userId;
                //p["user_id"] = Session.UserProfile.LocalID;
                AllApplicants postings = factory.SelectSingleDocument(p) as AllApplicants;
                return postings.data;
            }
        }

        public Posting LoadPosting(string postingId, string shortName)
        {
            DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("Posting");
            QueryParams p = factory.NewQueryParams();

            if (shortName != null && !shortName.Equals(String.Empty))
            {
                p["ShortName"] = shortName;
                p["docID"] = null;
            }
            else
                p["docID"] = postingId;

            return factory.SelectSingleDocument(p) as Posting;
        }

        public Posting CreatePosting()
        {
            DocumentFactory factory = WSApplication.Application.ActivityRoot.GetDocumentFactory("Posting");
            return (Posting)factory.CreateDocument();
        }
    }
}