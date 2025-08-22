using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    // Service for fetching cryptocurrency data from Bitstamp API
    public class BitstampApiService : ICryptoApiService
    {
        // Name of the exchange, used for display and analysis
        public string ExchangeName => "Bitstamp";

        private readonly HttpClient _httpClient;

        // Constructor accepts an HttpClient instance via dependency injection
        public BitstampApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // List of supported cryptocurrency symbols for Bitstamp
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

        // Fetches the current bid and ask price for a given symbol from Bitstamp
        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                // Normalize the symbol to uppercase for display and lowercase for API URL
                var sym = symbol.Trim().ToUpperInvariant();

                // Check if the symbol is supported by this service
                if (!SupportedSymbols.Contains(symbol.Trim().ToLowerInvariant()))
                    return CryptoResult.Fail($"Unsupported symbol: {sym}");

                // Bitstamp public REST API endpoint for ticker data
                string url = $"https://www.bitstamp.net/api/v2/ticker/{symbol}usd/";

                // Send GET request to the API and parse the JSON object response
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                // Validate that required fields exist in the response
                if (json["last"] == null || json["bid"] == null || json["ask"] == null)
                    return CryptoResult.Fail("Invalid response from Bitstamp");

                // Extract bid and ask prices from the JSON response
                decimal bid = json["bid"].Value<decimal>();
                decimal ask = json["ask"].Value<decimal>();

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
                return CryptoResult.Fail($"Error fetching data from Bitstamp: {ex.Message}");
            }
        }
    }
}
