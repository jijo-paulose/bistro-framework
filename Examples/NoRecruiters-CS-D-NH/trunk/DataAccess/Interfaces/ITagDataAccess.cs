using System.Collections.Generic;
namespace NoRecruiters.DataAccess
{
    /// <summary>
    /// Data access for tag data
    /// </summary>
    public interface ITagDataAccess
    {
        /// <summary>
        /// Gets a ranked list of the top <code>count</code> tags.
        /// </summary>
        /// <param name="count">The number of tags to retrieve.</param>
        /// <returns></returns>
        IList<Posting.tag> GetRankedTags(int count);


        /// <summary>
        /// Parses the tag string and places it into a posting as distinct 
        /// tag nodes.
        /// </summary>
        /// <param name="posting">The posting.</param>
        /// <param name="tags">The tags.</param>
        void LoadTags(Posting posting, string tags);
    }
}
