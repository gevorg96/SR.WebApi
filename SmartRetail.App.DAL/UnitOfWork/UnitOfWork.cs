using System;
using System.Data;

namespace SmartRetail.App.DAL.UnitOfWork
{
    public sealed class UnitOfWork: IUnitOfWork
    {
        public IDbConnection Connection { get; }

        public IDbTransaction Transaction { get; private set; }

        public Guid Id { get; }

        public UnitOfWork(IDbConnection connection)
        {
            Id = Guid.NewGuid();
            Connection = connection;
        }

        public void Begin()
        {
            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction.Commit();
            Dispose();
        }

        public void RollBack()
        {
            Transaction.Rollback();
            Dispose();
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Transaction = null;
        }

    }
}
