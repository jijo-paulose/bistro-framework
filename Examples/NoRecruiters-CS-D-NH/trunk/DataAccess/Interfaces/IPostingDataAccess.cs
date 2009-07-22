using System.Collections.Generic;
using System;
namespace NoRecruiters.DataAccess
{
    /// <summary>
    /// Data access for postings
    /// </summary>
    public interface IPostingDataAccess
    {
        /// <summary>
        /// Retrieves a list of postings with applicants. This method will either retrieve a single posting
        /// or all postings for a given user, based on which of the two parameters is supplied
        /// </summary>
        /// <param name="postingId">The posting id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        IList<Posting> ApplicantSearch(string postingId, string userId);

        /// <summary>
        /// Retrieves a posting. This method will load either by posting id, or by short name. Posting id
        /// will get priority, if present.
        /// </summary>
        /// <param name="postingId">The posting id.</param>
        /// <param name="shortName">The short name.</param>
        /// <returns></returns>
        Posting LoadPosting(string postingId, string shortName);
        
        /// <summary>
        /// Populates the posting with the supplied data.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="posting">The posting.</param>
        /// <param name="contentType">Type of the content.</param>
        void PopulatePosting(string contents, string tags, Posting posting, ContentType contentType);

        /// <summary>
        /// Retrieves a list of postings that satisfy the given query, tag list and content type. 
        /// query and/or tags can be null
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        IList<Posting> PostingSearch(string query, string tags, ContentType contentType);

        /// <summary>
        /// Saves the posting.
        /// </summary>
        /// <param name="posting">The posting.</param>
        void SavePosting(Posting posting);

        /// <summary>
        /// Saves the user action.
        /// </summary>
        /// <param name="action">The action.</param>
        void SaveUserAction(UserAction action);

        /// <summary>
        /// Retrieves a list of all postings for a given user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="published">if set to <c>true</c> only published postings will be retrieved. 
        ///     If <c>false</c>, only unpublished.</param>
        /// <returns></returns>
        IList<Posting> UserPostingSearch(string userId, bool published);

        /// <summary>
        /// Creates a new posting and populates defaults for nullable values.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <returns></returns>
        Posting CreatePosting(UserProfile currentUser);
    }
}