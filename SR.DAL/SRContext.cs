using Microsoft.EntityFrameworkCore;
using SR.DAL.Entities;
using SR.DAL.Entities.Auth;

namespace SR.DAL
{
    public class SRContext: DbContext
    {
        private readonly string _connString;
        public DbSet<UserProfile> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Cost> Costs { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpensesDetail> ExpensesDetails { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<InvoiceStock> InvoiceStocks { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Unit> Units { get; set; }

        public SRContext(string connString)
        {
            _connString = connString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserProfile>()
                .Property(x => x.Id)
                .HasColumnName("UserId");
            modelBuilder
                .Entity<UserProfile>()
                .HasIndex(x => x.UserName);
            modelBuilder
               .Entity<UserProfile>()
               .HasOne(x => x.Business)
               .WithMany(x => x.UserProfiles)
               .HasForeignKey(x => x.BusinessId);

            modelBuilder.Entity<Product>().Property(u => u.Id).HasColumnName("ProductId");
            modelBuilder.Entity<Product>().HasIndex(u => u.Name);



        }
    }
}
