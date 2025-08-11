
using Blocentra_3.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Blocentra_3.Services
{
    class BitfinexApiService : ICryptoApiService
    {
        public string ExchangeName => "Bitfinex";
        private readonly HttpClient _httpClient;

        public BitfinexApiService(HttpClient httpClient)
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
                var sym = symbol.Trim().ToUpperInvariant();

                if (!SupportedSymbols.Contains(sym))
                    return CryptoResult.Fail($"Неподдерживаемый символ: {symbol}");

                string url = $"https://api-pub.bitfinex.com/v2/ticker/t{sym}USD";

                var response = await _httpClient.GetStringAsync(url);
                var json = JArray.Parse(response);

                if (json.Count < 7)
                    return CryptoResult.Fail("Некорректный ответ от Bitfinex");

                decimal bid = json[0].Value<decimal>();
                decimal ask = json[2].Value<decimal>();
                //decimal lastPrice = json[6].Value<decimal>();

                var Currency = new CryptoCurrency
                {
                    Symbol = sym,
                    BidPrice = bid,
                    AskPrice = ask,
                    //PriceUsd = lastPrice,
                    ExchangeName = ExchangeName
                };

                return CryptoResult.Ok(Currency);


            }
            catch (Exception ex) 
            { 
                return CryptoResult.Fail($"Ошибка при запросе Bitfinex: {ex.Message}");
            }
        }
    }
}
