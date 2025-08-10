using Blocentra_3.Models;
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

                if (!SupportedSymbols.Contains(symbol.ToLowerInvariant())) {
            }
        }
    }
}
