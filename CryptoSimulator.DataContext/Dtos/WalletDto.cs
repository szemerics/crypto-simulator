using CryptoSimulator.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Dtos
{
    public class WalletDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public decimal Balance { get; set; }

        public ICollection<PortfolioWalletDto> Portfolios { get; set; }
    }
}
