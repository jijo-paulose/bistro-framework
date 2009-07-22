using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Cfg;
using NHibernate;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;

namespace NoRecruiters.DataAccess.NHibernate
{
    /// <summary>
    /// Centralizes configuration of NHibernate-specific components
    /// </summary>
    public class NHibernateManager
    {
        private static NHibernateManager instance = new NHibernateManager();

        public static NHibernateManager Instance { get { return instance; } }

        Configuration config = new Configuration();

        ISessionFactory sessionFactory;

        public void Initialize()
        {
            config.SetProperty("current_session_context_class", "web");
            config.AddAssembly("NoRecruiters");

            sessionFactory = config.BuildSessionFactory();
        }

        /// <summary>
        /// Gets a new session.
        /// </summary>
        /// <returns></returns>
        public ISession GetNewSession()
        {
            return sessionFactory.OpenSession();
        }

        /// <summary>
        /// Gets the session factory.
        /// </summary>
        /// <returns></returns>
        public ISessionFactory GetFactory()
        {
            return sessionFactory;
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        /// <returns></returns>
        public ISession GetSession()
        {
            return sessionFactory.GetCurrentSession();
        }
    }
}
