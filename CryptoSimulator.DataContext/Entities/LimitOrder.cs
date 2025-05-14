using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSimulator.DataContext.Entities
{
    public enum LimitOrderType
    {
        Buy,
        Sell
    }

    public enum LimitOrderStatus
    {
        Active,
        Completed,
        Cancelled,
        Expired
    }

    public class LimitOrder : AbstractEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int CryptoCurrencyId { get; set; }
        public CryptoCurrency CryptoCurrency { get; set; }
        public LimitOrderType OrderType { get; set; }
        public LimitOrderStatus Status { get; set; }
        public decimal Quantity { get; set; }
        public decimal LimitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
