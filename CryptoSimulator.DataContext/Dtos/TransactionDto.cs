using CryptoSimulator.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Dtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CryptoCurrencySymbol { get; set; }
        public string TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class TransactionCreateDto
    {
        public int CryptoCurrencyId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class TransactionDetailedDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CryptoCurrencySymbol { get; set; }
        public decimal PriceAtTransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
