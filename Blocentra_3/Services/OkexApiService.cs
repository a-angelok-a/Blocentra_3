using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Service to fetch cryptocurrency prices from the Okex exchange.
    /// Implements <see cref="ICryptoApiService"/> interface.
    /// </summary>
    public class OkexApiService : ICryptoApiService
    {
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string ExchangeName => "Okex";

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OkexApiService"/> class.
        /// </summary>
        /// <param name="httpClient">Injected HttpClient for API requests.</param>
        public OkexApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Supported cryptocurrency symbols on Okex.
        /// </summary>
        private static readonly HashSet<string> SupportedSymbols = new()
        {
            "BTC",
            "ETH",
            "USDT",
            "BNB",
            "ADA"
        };

        /// <summary>
        /// Retrieves the latest price information for the specified cryptocurrency symbol.
        /// </summary>
        /// <param name="symbol">The symbol of the cryptocurrency (e.g., BTC, ETH).</param>
        /// <returns>A <see cref="CryptoResult"/> containing either the fetched currency data or an error message.</returns>
        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                var sym = symbol.ToUpper();

                // Validate symbol support
                if (!SupportedSymbols.Contains(sym))
                    return CryptoResult.Fail($"Unsupported symbol: {symbol}");

                string url = $"https://www.okx.com/api/v5/market/ticker?instId={sym}-USDT";

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                // Validate JSON response structure
                if (json.Count < 2)
                    return CryptoResult.Fail("Data not found");

                var dataArray = json["data"] as JArray;
                if (dataArray == null || dataArray.Count == 0)
                    return CryptoResult.Fail("Data not found");

                decimal bidPrice = dataArray[0]["bidPx"].ToObject<decimal>();
                decimal askPrice = dataArray[0]["askPx"].ToObject<decimal>();

                var currency = new CryptoCurrency
                {
                    Symbol = sym,
                    BidPrice = bidPrice,
                    AskPrice = askPrice,
                    ExchangeName = ExchangeName
                };

                return CryptoResult.Ok(currency);
            }
            catch (Exception ex)
            {
                // Catch network or parsing exceptions
                return CryptoResult.Fail($"Error fetching data from Okex: {ex.Message}");
            }
        }
    }
}
