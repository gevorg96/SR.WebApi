using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SR.Application.Persistence
{
    public interface ISrContext
    {
        DbSet<Domain.User> Users { get; set; }
        DbSet<Domain.Business> Businesses { get; set; }
        DbSet<Domain.Shop> Shops { get; set; }
        DbSet<Domain.Supplier> Suppliers { get; set; }
        DbSet<Domain.UnitOfMeasure> UnitOfMeasures { get; set; }
        DbSet<Domain.Expense> Expenses { get; set; }
        DbSet<Domain.ExpenseType> ExpenseTypes { get; set; }
        DbSet<Domain.ExpenseItem> ExpenseItems { get; set; }
        DbSet<Domain.Folder> Folders { get; set; }
        DbSet<Domain.Product> Products { get; set; }
        DbSet<Domain.ProductProperty> ProductProperties { get; set; }
        DbSet<Domain.Image> Images { get; set; }
        DbSet<Domain.Order> Orders { get; set; }
        DbSet<Domain.OrderItem> OrderItems { get; set; }
        DbSet<Domain.Bill> Bills { get; set; }
        DbSet<Domain.BillItem> BillItems { get; set; }
        DbSet<Domain.Stock> Stocks { get; set; }
        DbSet<Domain.OrderStock> OrderStocks { get; set; }
        DbSet<ProductView> ProductView { get; set; }
        ValueTask<EntityEntry> AddAsync(object entity, CancellationToken token);

        DatabaseFacade Database { get; }
        
        Task<int> SaveChangesAsync(CancellationToken token);
    }
}