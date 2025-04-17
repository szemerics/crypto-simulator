namespace CryptoSimulator.DataContext.Entities
{
    public class PriceHistory : AbstractEntity
    {
        public int CryptoCurrencyId { get; set; }
        public CryptoCurrency CryptoCurrency { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}