using Microsoft.EntityFrameworkCore;
using SR.DAL.Repository.Interfaces;
using System;

namespace SR.DAL.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        int Commit();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
