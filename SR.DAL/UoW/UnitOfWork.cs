using SR.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SR.DAL.UoW
{
    class UnitOfWork : IUnitOfWork
    {
        public int Commit()
        {
            throw new NotImplementedException();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
