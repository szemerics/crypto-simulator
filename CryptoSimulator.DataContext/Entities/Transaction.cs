using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Entities
{
    public class Transaction : AbstractEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int CryptoCurrencyId { get; set; }
        public CryptoCurrency CryptoCurrency { get; set; }
        public string TransactionType { get; set; } // Buy or Sell
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}
