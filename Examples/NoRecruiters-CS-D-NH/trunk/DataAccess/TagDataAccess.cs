using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using NHibernate;
using NoRecruiters.DataAccess.NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Text;

namespace NoRecruiters.DataAccess
{
    /// <summary>
    /// Data access class for tags
    /// </summary>
    public class TagDataAccess : ITagDataAccess
    {
        static TagDataAccess instance = new TagDataAccess();

        private TagDataAccess() { }

        private IEnumerable<string> ParseAndDedupeTags(string tags)
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

        public static ITagDataAccess Instance { get { return instance; } }

        public void LoadTags(Posting posting, string tags)
        {
            if (tags == null)
                return;

            Dictionary<string, Posting.tag> reference = new Dictionary<string, Posting.tag>();
            List<Posting.tag> dupList = new List<Posting.tag>();
            foreach (Posting.tag node in posting.tags)
            {
                if (reference.ContainsKey(node.SafeText))
                {
                    dupList.Add(node);
                    continue;
                }

                reference[node.SafeText] = node;
            }

            foreach (string tagText in ParseAndDedupeTags(tags))
            {
                var safeText = StringHandler.Instance.ReplaceAll(tagText);

                if (reference.ContainsKey(safeText))
                {
                    reference.Remove(safeText);
                    continue;
                }

                Posting.tag newTag = new Posting.tag();
                newTag.TagText = tagText;
                newTag.SafeText = safeText;
                newTag.Posting = posting;

                posting.tags.Add(newTag);
            }

            ISession session = NHibernateManager.Instance.GetSession();

            // remove deleted tags
            foreach (Posting.tag tag in reference.Values)
            {
                session.Delete(tag);
                posting.tags.Remove(tag);
            }

            // remove dup tags
            foreach (Posting.tag tag in dupList)
            {
                session.Delete(tag);
                posting.tags.Remove(tag);
            }
        }

        public IList<Posting.tag> GetRankedTags(int count)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            return session.CreateCriteria(typeof(Posting.tag))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.RowCount(), "Id")
                    .Add(Projections.GroupProperty("TagText"), "TagText")
                    .Add(Projections.GroupProperty("SafeText"), "SafeText"))
                .AddOrder(Order.Desc("Id"))
                .SetMaxResults(count)
                .SetResultTransformer(Transformers.AliasToBean(typeof(Posting.tag)))
                .List<Posting.tag>();
        }
    }
}
