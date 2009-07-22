using System;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using NoRecruiters.DataAccess;
using NoRecruiters.DataAccess.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace NoRecruiters.DataAccess
{


    /// <summary>
    /// Summary description for PostingHelper
    /// </summary>
    public class PostingDataAccess : IPostingDataAccess
    {
        const int shortTextLength = 500;
        const int shortNameLength = 100;

        static PostingDataAccess instance = new PostingDataAccess();
        static string htmlPattern = @"<(.|\n)*?>";

        public static IPostingDataAccess Instance { get { return instance; } }

        private PostingDataAccess() { }

        protected string StripSummaryText(string htmlSummary)
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
            if (posting.CreatedOn == null)
            {
                posting.ContentType = contentType;
                posting.CreatedOn = DateTime.Now;
            }

            if (posting.Contents == null)
                posting.Contents = new Contents();

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
                string salt = new Random().Next(1000000).ToString();
                string shortName = StringHandler.Instance.ReplaceAll(posting.Heading);
                posting.ShortName = 
                    shortName.Substring(
                        0, 
                        Math.Min(shortNameLength - salt.Length, shortName.Length)
                    ) + salt;
            }

            posting.LastModifiedOn = DateTime.Now;

            TagDataAccess.Instance.LoadTags(posting, tags);
        }

        private IList<Posting> EmptySearch(ContentType contentType)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            return session.CreateCriteria(typeof(Posting))
                .Add(Expression.Eq("ContentType", contentType))
                .Add(Expression.Eq("Deleted", false))
                .Add(Expression.Eq("Published", true))
                .List<Posting>();
        }

        public IList<Posting> PostingSearch(string query, string tags, ContentType contentType)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            IList<Posting> res = session.GetNamedQuery("advanced_posting_search")
                .SetEnum("content_type", contentType)
                .SetString("tag_list", tags)
                .SetString("query", String.IsNullOrEmpty(query) ? null:query)
                .SetInt16("precision", 2).List<Posting>();

            return res;
        }

        public IList<Posting> UserPostingSearch(string userId, bool published)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            return session.CreateCriteria(typeof(Posting))
                .Add(Expression.Eq("Published", published))
                .Add(Expression.Eq("Deleted", false))
                .CreateCriteria("User")
                    .Add(Expression.Eq("Id", userId))
                .List<Posting>();
        }

        public IList<Posting> ApplicantSearch(string postingId, string userId)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            var criteria = session.CreateCriteria(typeof(Posting))
                .Add(Expression.Eq("Published", true))
                .Add(Expression.Eq("Deleted", false));

            if (!String.IsNullOrEmpty(postingId))
                criteria = criteria
                    .Add(Expression.Eq("Id", postingId));
            else
                criteria = criteria
                    .CreateCriteria("User")
                        .Add(Expression.Eq("Id", userId));

            return criteria.List<Posting>();
        }

        public Posting LoadPosting(string postingId, string shortName)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            if (!String.IsNullOrEmpty(postingId))
                return session.Get<Posting>(postingId);

            var postingList = session.CreateCriteria(typeof(Posting))
                .Add(Expression.Eq("ShortName", shortName))
                .List<Posting>();

            if (postingList.Count != 1)
                return null;

            return postingList[0];
        }

        public void SavePosting(Posting posting)
        {
            posting.LastModifiedOn = DateTime.Now;

            ISession session = NHibernateManager.Instance.GetSession();

            if (posting.Contents != null)
                session.SaveOrUpdate(posting.Contents);
            session.SaveOrUpdate(posting);

            session.Flush();
        }

        /// <summary>
        /// Saves the user action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="dataContext">The data context.</param>
        public void SaveUserAction(UserAction action)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            session.SaveOrUpdate(action);

            session.Flush();
        }

        public Posting CreatePosting(UserProfile currentUser)
        {
            var posting = new Posting();
            posting.User = currentUser;
            posting.tags = new List<Posting.tag>();
            posting.Views = 0;
            posting.Deleted = false;
            posting.Flagged = false;
            posting.Published = false;
            posting.Active = false;
            posting.CreatedOn = DateTime.Now;

            return posting;
        }
    }
}