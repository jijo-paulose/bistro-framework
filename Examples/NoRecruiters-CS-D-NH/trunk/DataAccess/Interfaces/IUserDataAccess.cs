namespace NoRecruiters.DataAccess
{
    /// <summary>
    /// Data access for user data
    /// </summary>
    public interface IUserDataAccess
    {
        /// <summary>
        /// Loads the user.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        UserProfile LoadUser(string name);

        /// <summary>
        /// Saves the user.
        /// </summary>
        /// <param name="user">The user.</param>
        void SaveUser(UserProfile user);
    }
}
