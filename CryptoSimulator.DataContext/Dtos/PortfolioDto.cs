using CryptoSimulator.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Dtos
{
    //public class PortfolioDto
    //{
    //    public int Id { get; set; }
    //    // Which wallet it belongs to
    //    public int WalletId { get; set; }
    //    public Wallet Wallet { get; set; }

    //    // What does the portfolio have
    //    public int CryptoCurrencyId { get; set; }
    //    public CryptoCurrency CryptoCurrency { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal PurchasePrice { get; set; }
    //}

    public class PortfolioWalletDto
    {
        public string CryptoCurrencySymbol { get; set; }
        public decimal Quantity { get; set; }
    }

}
