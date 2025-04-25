using CryptoSimulator.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Context
{
    public class CryptoDbContext : DbContext
    {
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options)
        {

        }
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Role> Roles { get; set; }
        public DbSet<Entities.CryptoCurrency> CryptoCurrencies { get; set; }
        public DbSet<Entities.Transaction> Transactions { get; set; }
        public DbSet<Entities.Portfolio> Portfolios { get; set; }
        public DbSet<Entities.Wallet> Wallets { get; set; }
        public DbSet<Entities.PriceHistory> PriceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Filtering to filter out soft-deleted lines
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(AbstractEntity).IsAssignableFrom(t.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetIsDeletedFilter(entityType.ClrType));
            }

            // Role Config
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );

        }

        private static LambdaExpression GetIsDeletedFilter(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(parameter, "isDeleted");
            var constant = Expression.Constant(false);
            var equals = Expression.Equal(property, constant);
            return Expression.Lambda(equals, parameter);
        }

    }
}
