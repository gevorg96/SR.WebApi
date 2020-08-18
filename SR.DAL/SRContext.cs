using Microsoft.EntityFrameworkCore;
using SR.DAL.Entities;
using System;

namespace SR.DAL
{
    public class SRContext: DbContext
    {
        private readonly string _connString;

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
            modelBuilder.Entity<Product>().HasIndex(u => u.ProductName);

        }
    }
}
