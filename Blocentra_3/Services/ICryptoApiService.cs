using Blocentra_3.Models;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Defines the contract for cryptocurrency API services.
    /// Each implementation should provide methods to fetch bid/ask prices for supported cryptocurrencies.
    /// </summary>
    public interface ICryptoApiService
    {
        /// <summary>
        /// The name of the exchange associated with this API service.
        /// Used for labeling returned currency data.
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// Asynchronously retrieves the bid and ask prices for a given cryptocurrency symbol.
        /// </summary>
        /// <param name="symbol">The cryptocurrency symbol, e.g., "BTC" or "ETH".</param>
        /// <returns>
        /// A <see cref="CryptoResult"/> containing a <see cref="CryptoCurrency"/> object if successful,
        /// or an error message if the request fails.
        /// </returns>
        Task<CryptoResult> GetCurrencyAsync(string symbol);
    }
}
