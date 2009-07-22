using System;
namespace NoRecruiters.DataAccess.NHibernate
{
    public interface IDataContext: IDisposable
    {
        void Cleanup();
        void Setup();
    }
}
