using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public DbSet<Entities.CryptoCurrency> CryptoCurrencies { get; set; }
        public DbSet<Entities.Transaction> Transactions { get; set; }
        public DbSet<Entities.Portfolio> Portfolios { get; set; }
        public DbSet<Entities.Wallet> Wallets { get; set; }
        public DbSet<Entities.PriceHistory> PriceHistories { get; set; }

    }
}
