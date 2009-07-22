using System;
using System.Data;
using System.Configuration;
using NoRecruiters.DataAccess;
using NHibernate;
using NoRecruiters.DataAccess.NHibernate;
using NHibernate.Criterion;

namespace NoRecruiters.DataAccess
{
    /// <summary>
    /// Summary description for UserHelper
    /// </summary>
    public class UserDataAccess : IUserDataAccess
    {
        private static UserDataAccess instance = new UserDataAccess();

        private UserDataAccess() { }

        public static IUserDataAccess Instance { get { return instance; } }

        public UserProfile LoadUser(string name)
        {
            ISession session = NHibernateManager.Instance.GetSession();

            var userList = session.CreateCriteria(typeof(UserProfile))
                .Add(Expression.Eq("UserName", name))
                .List<UserProfile>();

            if (userList.Count != 1)
                return null;

            session.Evict(userList);
            session.Evict(userList[0]);

            return userList[0];
        }

        public void SaveUser(UserProfile user)
        {
            ISession session = NHibernateManager.Instance.GetSession();
            session.SaveOrUpdate(user);

            session.Flush();
        }
    }
}