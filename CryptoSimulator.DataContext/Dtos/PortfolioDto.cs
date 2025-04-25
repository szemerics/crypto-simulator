using CryptoSimulator.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Dtos
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }

    public class PortfolioWalletDto
    {
        public string CryptoCurrencySymbol { get; set; }
        public decimal Quantity { get; set; }
    }

    public class PortfolioProfitDto
    {
        public string CryptoCurrencySymbol { get; set; }
        public decimal Quantity { get; set; }
        public decimal Profit { get; set; }
    }

    public class PortfolioProfitSummaryDto
    {
        public decimal TotalProfit { get; set; }
        public List<PortfolioProfitDto> Profits { get; set; }
    }

    public class PortfolioTotalProfitDto
    {
        public decimal TotalProfit { get; set; }
    }

}
