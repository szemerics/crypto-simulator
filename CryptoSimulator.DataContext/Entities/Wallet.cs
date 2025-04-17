using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Entities
{
    public class Wallet : AbstractEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Balance { get; set; }

        public ICollection<Portfolio> Portfolios { get; set; }
    }
}
