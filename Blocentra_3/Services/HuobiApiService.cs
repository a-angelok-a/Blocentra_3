using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    public class HuobiApiService : ICryptoApiService
    {
        public string ExchangeName => "Huobi";
        private readonly HttpClient _httpClient;

        public HuobiApiService(HttpClient httpClient)
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

                if (!SupportedSymbols.Contains(symbol.ToLowerInvariant()))
                    return CryptoResult.Fail($"Неподдерживаемый символ: {symbol}");

                var url = $"https://api.huobi.pro/market/detail/merged?symbol={symbol}usdt";

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["status"]?.ToString() != "ok" || json["tick"] == null)
                    return CryptoResult.Fail("Некорректный ответ от Huobi");

                decimal bid = json["tick"]["bid"]?[0]?.ToObject<decimal>() ?? 0m;
                decimal ask = json["tick"]["ask"]?[0]?.ToObject<decimal>() ?? 0m;

                var Currency = new CryptoCurrency
                {
                    Symbol = sym,
                    //riceUsd = lastPrice,
                    BidPrice = bid,
                    AskPrice = ask,
                    ExchangeName = ExchangeName
                };

                return CryptoResult.Ok(Currency);

            }
            catch (Exception ex)
            { return CryptoResult.Fail($"Ошибка при запросе Huobi: {ex.Message}"); }
        }
    }
}
