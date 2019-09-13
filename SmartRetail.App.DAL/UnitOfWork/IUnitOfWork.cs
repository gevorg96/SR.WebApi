using System;
using System.Data;

namespace SmartRetail.App.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Begin();
        void Commit();
        void RollBack();
    }
}
