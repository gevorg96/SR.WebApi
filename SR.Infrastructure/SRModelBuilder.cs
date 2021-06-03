using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;
using SR.Domain;

namespace SR.Infrastructure
{
    public static class SrModelBuilder
    {
        public static ModelBuilder BuildPrimaryKeys(this ModelBuilder builder)
        {
            builder.Entity<User>().HasKey(x => x.Id);
            builder.Entity<Business>().HasKey(x => x.Id);
            builder.Entity<Shop>().HasKey(x => x.Id);
            builder.Entity<Supplier>().HasKey(x => x.Id);
            builder.Entity<UnitOfMeasure>().HasKey(x => x.Id);
            builder.Entity<Expense>().HasKey(x => x.Id);
            builder.Entity<ExpenseType>().HasKey(x => x.Id);
            builder.Entity<ExpenseItem>().HasKey(x => x.Id);
            builder.Entity<Folder>().HasKey(x => x.Id);
            builder.Entity<Product>().HasKey(x => x.Id);
            builder.Entity<ProductProperty>().HasKey(x => x.Id);
            builder.Entity<Image>().HasKey(x => x.Uid);
            builder.Entity<Order>().HasKey(x => x.Id);
            builder.Entity<OrderItem>().HasKey(x => x.Id);
            builder.Entity<Bill>().HasKey(x => x.Id);
            builder.Entity<BillItem>().HasKey(x => x.Id);
            builder.Entity<Stock>().HasKey(x => x.Id);
            builder.Entity<OrderStock>().HasKey(x => x.Id);

            return builder;
        }

        public static ModelBuilder BuildProductView(this ModelBuilder builder)
        {
            builder.Entity<ProductView>().HasNoKey();
            builder.Entity<ProductView>().ToView("ProductView");
            
            return builder;
        }
    }
}