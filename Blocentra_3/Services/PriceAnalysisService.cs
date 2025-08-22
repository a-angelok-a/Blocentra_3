using Blocentra_3.Models;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Service for analyzing cryptocurrency prices across multiple exchanges.
    /// Implements <see cref="IPriceAnalysisService"/>.
    /// </summary>
    public class PriceAnalysisService : IPriceAnalysisService
    {
        /// <summary>
        /// Returns the cryptocurrency with the lowest ask price from a collection of results.
        /// </summary>
        /// <param name="results">A collection of <see cref="CryptoResult"/> objects.</param>
        /// <returns>
        /// The <see cref="CryptoCurrency"/> with the lowest ask price. 
        /// If no valid data is available, returns a default "N/A" currency.
        /// </returns>
        public CryptoCurrency GetLowerPrice(IEnumerable<CryptoResult> results)
        {
            // Filter successful results and select the currency objects
            var currency = results
                .Where(r => r.IsSuccess)
                .Select(r => r.Currency)
                .MinBy(c => c.AskPrice); // Find the one with minimum ask price

            // Return a default value if no currency is found
            return currency ?? new CryptoCurrency
            {
                Symbol = "N/A",
                AskPrice = 0m,
                ExchangeName = "None"
            };
        }

        /// <summary>
        /// Returns the cryptocurrency with the highest bid price from a collection of results.
        /// </summary>
        /// <param name="results">A collection of <see cref="CryptoResult"/> objects.</param>
        /// <returns>
        /// The <see cref="CryptoCurrency"/> with the highest bid price. 
        /// If no valid data is available, returns a default "N/A" currency.
        /// </returns>
        public CryptoCurrency GetHighestPrice(IEnumerable<CryptoResult> results)
        {
            // Filter successful results and select the currency objects
            var currency = results
                .Where(r => r.IsSuccess)
                .Select(r => r.Currency)
                .MaxBy(c => c.BidPrice); // Find the one with maximum bid price

            // Return a default value if no currency is found
            return currency ?? new CryptoCurrency
            {
                Symbol = "N/A",
                BidPrice = 0m,
                ExchangeName = "None"
            };
        }
    }
}
