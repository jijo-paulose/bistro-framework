using System;
using NHibernate;

namespace NoRecruiters.DataAccess.NHibernate
{
    /// <summary>
    /// Interface for encapsulating ORM/DAL-specific context information
    /// </summary>
    public class NHDataContext: IDataContext
    {
        ISession session;

        public void Setup()
        {
            session = NHibernateManager.Instance.GetNewSession();
        }

        public void Cleanup()
        {
            if (session.Transaction != null &&
                session.Transaction.IsActive)
            {
                session.Transaction.Rollback();
            }
            else
                session.Flush();

            session.Close();
        }

        public void Dispose()
        {
            Cleanup();
        }

        public ISession CurrentSession()
        {
            return session;
        }
    }
}
