namespace NoRecruiters 
{
    /// <summary>
    /// Represents a posting contents. This class is defined to allow lazy-loading for
    /// posting content.
    /// </summary>
    public class Contents {
        public virtual string ContentsText { get; set; }
        public virtual string Id { get; set; }
    }
}
