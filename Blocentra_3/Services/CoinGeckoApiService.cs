using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    // Service for fetching cryptocurrency data from CoinGecko API
    public class CoinGeckoApiService : ICryptoApiService
    {
        // Name of the exchange
        public string ExchangeName => "CoinGecko";

        private readonly HttpClient _httpClient;

        // Constructor accepts an HttpClient instance via dependency injection
        public CoinGeckoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Mapping of common symbols to CoinGecko coin IDs
        private static readonly Dictionary<string, string> SymbolToCoinGeckoId = new()
        {
            { "btc", "bitcoin" },
            { "eth", "ethereum" },
            { "usdt", "tether" },
            { "bnb", "binancecoin" },
            { "ada", "cardano" },
        };

        // Fetches the current price for a given symbol from CoinGecko
        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                // Check if the symbol is supported and get its CoinGecko ID
                if (!SymbolToCoinGeckoId.TryGetValue(symbol.ToLower(), out var id))
                {
                    return CryptoResult.Fail($"Unsupported symbol: {symbol}");
                }

                // CoinGecko simple price API endpoint
                string url = $"https://api.coingecko.com/api/v3/simple/price?ids={id}&vs_currencies=usd";

                // Send GET request and parse the response
                string response = await _httpClient.GetStringAsync(url);
                return ParseResponse(symbol, response);
            }
            catch (Exception ex)
            {
                // Catch network or parsing errors and return as failed result
                return CryptoResult.Fail($"Error fetching data from CoinGecko: {ex.Message}");
            }
        }

        // Parses the JSON response and returns a CryptoResult
        private CryptoResult ParseResponse(string symbol, string jsonResponse)
        {
            var json = JObject.Parse(jsonResponse);

            // Validate that the symbol exists in the mapping
            if (!SymbolToCoinGeckoId.TryGetValue(symbol.ToLower(), out var id))
                return CryptoResult.Fail("Unknown cryptocurrency symbol");

            // Extract USD price if available
            if (json[id]?["usd"] != null)
            {
                decimal price = (decimal)json[id]["usd"];
                var currency = new CryptoCurrency
                {
                    Symbol = symbol.ToUpper(),
                    BidPrice = price,   // CoinGecko does not provide separate bid/ask, so we use the price
                    AskPrice = price,
                    ExchangeName = ExchangeName
                };
                return CryptoResult.Ok(currency);
            }

            // Return failure if data is missing
            return CryptoResult.Fail("Data not found.");
        }
    }
}
