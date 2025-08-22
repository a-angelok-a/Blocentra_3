using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Service for fetching cryptocurrency prices from the Huobi exchange.
    /// Implements the ICryptoApiService interface.
    /// </summary>
    public class HuobiApiService : ICryptoApiService
    {
        /// <summary>
        /// The exchange name, used for labeling returned currency data.
        /// </summary>
        public string ExchangeName => "Huobi";

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor accepting an HttpClient instance for API calls.
        /// </summary>
        /// <param name="httpClient">Shared HttpClient for network requests.</param>
        public HuobiApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Supported cryptocurrency symbols for this service
        private static readonly HashSet<string> SupportedSymbols = new()
        {
            "btc",
            "eth",
            "ltc",
            "xrp",
            "bch",
            "link",
            "eos",
            "ada"
        };

        /// <summary>
        /// Retrieves the bid and ask prices for a given cryptocurrency symbol from Huobi.
        /// </summary>
        /// <param name="symbol">Cryptocurrency symbol, e.g., "BTC".</param>
        /// <returns>CryptoResult containing the CryptoCurrency object or a failure message.</returns>
        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                var sym = symbol.Trim().ToUpperInvariant();

                // Check if the symbol is supported
                if (!SupportedSymbols.Contains(symbol.ToLowerInvariant()))
                    return CryptoResult.Fail($"Unsupported symbol: {symbol}");

                // Construct the Huobi API URL for merged market details
                var url = $"https://api.huobi.pro/market/detail/merged?symbol={symbol}usdt";

                // Send GET request
                var response = await _httpClient.GetStringAsync(url);

                // Parse the JSON response
                var json = JObject.Parse(response);

                // Validate response structure and status
                if (json["status"]?.ToString() != "ok" || json["tick"] == null)
                    return CryptoResult.Fail("Invalid response from Huobi");

                // Extract bid and ask prices from JSON safely
                decimal bid = json["tick"]["bid"]?[0]?.ToObject<decimal>() ?? 0m;
                decimal ask = json["tick"]["ask"]?[0]?.ToObject<decimal>() ?? 0m;

                // Create a CryptoCurrency instance with the retrieved data
                var Currency = new CryptoCurrency
                {
                    Symbol = sym,
                    BidPrice = bid,
                    AskPrice = ask,
                    ExchangeName = ExchangeName
                };

                return CryptoResult.Ok(Currency);
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully and return as a failure result
                return CryptoResult.Fail($"Error fetching data from Huobi: {ex.Message}");
            }
        }
    }
}
