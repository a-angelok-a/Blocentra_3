using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;


namespace Blocentra_3.Services
{
    public class CoinGeckoApiService : ICryptoApiService
    {
        public string ExchangeName => "CoinGecko";
        private readonly HttpClient _httpClient;

        public CoinGeckoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        private static readonly Dictionary<string, string> SymbolToCoinGeckoId = new()
        {
            { "btc", "bitcoin" },
            { "eth", "ethereum" },
            { "usdt", "tether" },
            { "bnb", "binancecoin" },
            { "ada", "cardano" },
        };
        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                if (!SymbolToCoinGeckoId.TryGetValue(symbol.ToLower(), out var id))
                {
                    return CryptoResult.Fail($"Неподдерживаемый символ: {symbol}");
                }
                string url = $"https://api.coingecko.com/api/v3/simple/price?ids={id}&vs_currencies=usd";

                string response = await _httpClient.GetStringAsync(url);
                return ParseResponse(symbol, response);
            }
            catch (Exception ex) 
            { 
                return CryptoResult.Fail($"Ошибка при запросе: {ex.Message}");
            }
        }
        private CryptoResult ParseResponse(string symbol, string jsonResponse)
        {
            var json = JObject.Parse(jsonResponse);
            if (!SymbolToCoinGeckoId.TryGetValue(symbol.ToLower(), out var id))
                return CryptoResult.Fail("Неизвестный символ криптовалюты");


            if (json[id]?["usd"] != null)
            {
                decimal price = (decimal)json[id]["usd"];
                var currency = new CryptoCurrency
                {
                    Symbol = symbol.ToUpper(),
                    BidPrice = price,   // нет bid в CoinGecko, поэтому ставим цену
                    AskPrice = price,
                    ExchangeName = ExchangeName
                };
                return CryptoResult.Ok(currency);
            }

            return CryptoResult.Fail("Данные не найдены.");
        }
    }
}
