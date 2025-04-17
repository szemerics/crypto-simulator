using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CryptoSimulator.DataContext.Entities
{
    public class CryptoCurrency : AbstractEntity
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal CurrentPrice { get; set; }

        public ICollection<PriceHistory> PriceHistories { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
