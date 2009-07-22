using System;
namespace NoRecruiters
{
    /// <summary>
    /// Enum of actions that can be taken against a posting
    /// </summary>
    public enum Actions
    {
        Flag,
        Spam,
        Recruiter
    }

    /// <summary>
    /// Represents a single user action against a posting
    /// </summary>
    public class UserAction
    {
        public virtual UserProfile User { get; set; }

        public virtual Actions Action { get; set; }

        public virtual string Comment { get; set; }

        public virtual DateTime? CreatedOn { get; set; }

        public virtual Posting Posting { get; set; }

        public virtual int Id { get; set; }
    }
}
