namespace CryptoSimulator.DataContext.Entities
{
    public class Portfolio : AbstractEntity
    {
        // Which wallet it belongs to
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }

        // What does the portfolio have
        public int CryptoCurrencyId { get; set; }
        public CryptoCurrency CryptoCurrency { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}