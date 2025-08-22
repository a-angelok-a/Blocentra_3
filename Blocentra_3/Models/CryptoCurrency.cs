namespace Blocentra_3.Models
{
    /// <summary>
    /// Represents a cryptocurrency trading pair or asset as retrieved from an exchange.
    /// Contains basic market data such as bid/ask prices and the exchange name.
    /// </summary>
    public class CryptoCurrency
    {
        /// <summary>
        /// The symbol or ticker of the cryptocurrency (e.g., "BTCUSDT", "ETHUSD").
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The highest price that a buyer is currently willing to pay for the asset.
        /// </summary>
        public decimal BidPrice { get; set; }

        /// <summary>
        /// The lowest price at which a seller is currently willing to sell the asset.
        /// </summary>
        public decimal AskPrice { get; set; }

        /// <summary>
        /// The name of the exchange providing this market data (e.g., "Binance", "Coinbase").
        /// </summary>
        public string ExchangeName { get; set; }
    }
}
