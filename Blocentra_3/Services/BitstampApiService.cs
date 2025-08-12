using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    public class BitstampApiService : ICryptoApiService
    {
        public string ExchangeName => "Bitstamp";

        private readonly HttpClient _httpClient;

        public BitstampApiService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

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

        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                var sym = symbol.Trim().ToUpperInvariant();

                if (!SupportedSymbols.Contains(symbol.Trim().ToLowerInvariant()))
                    return CryptoResult.Fail($"Неподдерживаемый символ: {sym}");

                string url = $"https://www.bitstamp.net/api/v2/ticker/{symbol}usd/";

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["last"] == null || json["bid"] == null || json["ask"] == null)
                    return CryptoResult.Fail("Некорректный ответ от Bitstamp");

                decimal bid = json["bid"].Value<decimal>();
                decimal ask = json["ask"].Value<decimal>();

                var Currency = new CryptoCurrency
                {
                    Symbol = sym,
                    BidPrice = bid,
                    AskPrice = ask,
                    ExchangeName = ExchangeName
                };

                return CryptoResult.Ok(Currency);

            }
            catch(Exception ex) 
            { return CryptoResult.Fail($"Ошибка при запросе Bitstamp: {ex.Message}"); }
        }
    }
}
