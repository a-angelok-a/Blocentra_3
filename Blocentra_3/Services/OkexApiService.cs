
using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Blocentra_3.Services
{
    public class OkexApiService : ICryptoApiService
    {
        public string ExchangeName => "Okex";
        private readonly HttpClient _httpClient;

        public OkexApiService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        private static readonly HashSet<string> SupportedSymbols = new()
        {
            "BTC",
            "ETH",
            "USDT",
            "BNB",
            "ADA"
        };

        public async Task<CryptoResult> GetCurrencyAsync(string symbol)
        {
            try
            {
                var sym = symbol.ToUpper();
                if (!SupportedSymbols.Contains(sym))
                    return CryptoResult.Fail($"Неподдерживаемый символ: {symbol}");

                string url = $"https://www.okx.com/api/v5/market/ticker?instId={sym}-USDT";

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);


                if (json.Count < 2)
                    return CryptoResult.Fail("Данные не найдены");

                var dataArray = json["data"] as JArray;
                if (dataArray == null || dataArray.Count == 0)
                    return CryptoResult.Fail("Данные не найдены");

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
                return CryptoResult.Fail($"Ошибка при запросе Okex: {ex.Message}");
            }
        }

    }
}
