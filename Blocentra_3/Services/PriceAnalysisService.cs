
using Blocentra_3.Models;

namespace Blocentra_3.Services
{
    public class PriceAnalysisService : IPriceAnalysisService
    {
        public CryptoCurrency GetLowerPrice(IEnumerable<CryptoResult> results)
        {
            var currency = results
            .Where(r => r.IsSuccess)
            .Select(r => r.Currency)
            .MinBy(c => c.AskPrice);

            return currency ?? new CryptoCurrency
            {
                Symbol = "N/A",
                AskPrice = 0m,
                ExchangeName = "None"
            };


        }

        public CryptoCurrency GetHighestPrice(IEnumerable<CryptoResult> results)
        {
            var currency = results
             .Where(r => r.IsSuccess)
             .Select(r => r.Currency)
             .MaxBy(c => c.BidPrice);

            return currency ?? new CryptoCurrency
            {
                Symbol = "N/A",
                BidPrice = 0m,
                ExchangeName = "None"
            };
        }
    }
}
