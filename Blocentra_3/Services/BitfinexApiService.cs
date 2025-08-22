using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    // Service for fetching cryptocurrency data from Bitfinex API
    public class BitfinexApiService : ICryptoApiService
    {
        // Name of the exchange, used for display and analysis
        public string ExchangeName => "Bitfinex";

        private readonly HttpClient _httpClient;

        // Constructor accepts an HttpClient instance via dependency injection
        public BitfinexApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // List of supported cryptocurrency symbols for this service
        private static readonly HashSet<string> SupportedSymbols = new()
        {
            "BTC",
            "ETH",
            "USDT",
            "BNB",
            "ADA"
        };

        // Fetches the current bid and ask price for a given symbol from Bitfinex
        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                // Normalize the symbol to uppercase and remove whitespace
                var sym = symbol.Trim().ToUpperInvariant();

                // Check if the symbol is supported by this service
                if (!SupportedSymbols.Contains(sym))
                    return CryptoResult.Fail($"Unsupported symbol: {symbol}");

                // Bitfinex public REST API endpoint for ticker data
                string url = $"https://api-pub.bitfinex.com/v2/ticker/t{sym}USD";

                // Send GET request to the API and parse the JSON array response
                var response = await _httpClient.GetStringAsync(url);
                var json = JArray.Parse(response);

                // Ensure the response contains enough elements
                if (json.Count < 7)
                    return CryptoResult.Fail("Invalid response from Bitfinex");

                // Extract bid and ask prices from the response array
                decimal bid = json[0].Value<decimal>();
                decimal ask = json[2].Value<decimal>();

                // Create a CryptoCurrency object with retrieved values
                var Currency = new CryptoCurrency
                {
                    Symbol = sym,
                    BidPrice = bid,
                    AskPrice = ask,
                    ExchangeName = ExchangeName
                };

                // Return a successful result containing the currency data
                return CryptoResult.Ok(Currency);
            }
            catch (Exception ex)
            {
                // Catch network or parsing errors and return as failed result
                return CryptoResult.Fail($"Error fetching data from Bitfinex: {ex.Message}");
            }
        }
    }
}
