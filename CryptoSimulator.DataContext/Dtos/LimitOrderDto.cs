using CryptoSimulator.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Dtos
{
    public class LimitOrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public string CryptoCurrencySymbol { get; set; }
        public decimal Quantity { get; set; }
        public decimal LimitPrice { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ExpireAt { get; set; }
    }

    public class LimitOrderCreateDto
    {
        public int CryptoCurrencyId { get; set; }
        public decimal Quantity { get; set; }
        public decimal LimitPrice { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
