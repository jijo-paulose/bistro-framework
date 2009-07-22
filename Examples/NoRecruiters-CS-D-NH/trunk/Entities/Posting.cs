using System;
using System.Collections.Generic;
using System.Text;

namespace NoRecruiters
{
    /// <summary>
    /// Posting object. Defines the data elements and basic helper operations on a posting.
    /// </summary>
    public class Posting {
        public const int MAX_TAG_LENGTH = 50;

        /// <summary>
        /// Publishes this instance.
        /// </summary>
        public virtual void Publish()
        {
            if (Published ?? false)
                return;

            if (Deleted ?? false)
                return;

            Published = true;
        }

        /// <summary>
        /// Suspends this instance.
        /// </summary>
        public virtual void Suspend()
        {
            if (!(Published ?? false))
                return;

            if (Deleted ?? false)
                return;

            Published = false;
        }

        /// <summary>
        /// Flags with the specified flag.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public virtual UserAction Flag(int flag, UserProfile user)
        {
            UserAction userAction = new UserAction();
            userAction.Id = flag;

            if (user != null)
                userAction.User = user;

            userAction.CreatedOn = System.DateTime.Now;
            userAction.Posting = this;

            return userAction;
        }

        /// <summary>
        /// Generates an application node for the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="comment">The comment.</param>
        public virtual void Apply(UserProfile user, string comment)
        {
            var application = new Posting.application();

            application.Comment = comment;
            application.SubmittedBy = user;
            application.SubmittedOn = DateTime.Now;
            application.SubmittedPosting = user.Posting;
            application.Posting = this;

            applications.Add(application);
        }

        /// <summary>
        /// Gets the tags as a comma-deliminted string.
        /// </summary>
        /// <value>The tags as string.</value>
        public virtual string TagsAsString
        {
            get
            {
                StringBuilder tagList = new StringBuilder();
                foreach (Posting.tag node in tags)
                {
                    if (tagList.Length > 0)
                        tagList.Append(", ");

                    tagList.Append(node.TagText);
                }

                return tagList.ToString();
            }
        }
        
        public virtual IList<tag> tags { get; set; }
        public virtual IList<application> applications { get; set; }
        public virtual UserProfile User { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? LastModifiedOn { get; set; }
        public virtual ContentType ContentType { get; set; }
        public virtual string Heading { get; set; }
        public virtual string ShortName { get; set; }
        public virtual string ShortText { get; set; }
        public virtual int? Views { get; set; }
        public virtual bool? Deleted { get; set; }
        public virtual bool? Flagged { get; set; }
        public virtual bool? Published { get; set; }
        public virtual bool? Active { get; set; }
        public virtual Contents Contents { get; set; }
        public virtual string Id { get; set; }

        /// <summary>
        /// Represents a single tag on this posting
        /// </summary>
        public class tag
        {
            public virtual int Id { get; set; }
            public virtual string TagText { get; set; }
            public virtual string SafeText { get; set; }
            public virtual Posting Posting
            { get; set; }
        }

        /// <summary>
        /// Represents a single application or contact request with for this posting
        /// </summary>
        public class application
        {
            public virtual Posting SubmittedPosting { get; set; }
            public virtual DateTime? SubmittedOn { get; set; }
            public virtual UserProfile SubmittedBy { get; set; }
            public virtual string Comment { get; set; }
            public virtual int Id { get; set; }
            public virtual Posting Posting { get; set; }
        }
    }
}
