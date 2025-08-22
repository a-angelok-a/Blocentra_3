using Blocentra_3.Models;
using System.Collections.Generic;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Defines a contract for analyzing cryptocurrency price data.
    /// Implementations are responsible for identifying the lowest and highest prices from a collection of results.
    /// </summary>
    public interface IPriceAnalysisService
    {
        /// <summary>
        /// Determines the cryptocurrency with the lowest price from a given set of results.
        /// </summary>
        /// <param name="results">A collection of <see cref="CryptoResult"/> objects to analyze.</param>
        /// <returns>
        /// A <see cref="CryptoCurrency"/> object representing the currency with the lowest price.
        /// Returns null if the collection is empty or contains no valid results.
        /// </returns>
        CryptoCurrency GetLowerPrice(IEnumerable<CryptoResult> results);

        /// <summary>
        /// Determines the cryptocurrency with the highest price from a given set of results.
        /// </summary>
        /// <param name="results">A collection of <see cref="CryptoResult"/> objects to analyze.</param>
        /// <returns>
        /// A <see cref="CryptoCurrency"/> object representing the currency with the highest price.
        /// Returns null if the collection is empty or contains no valid results.
        /// </returns>
        CryptoCurrency GetHighestPrice(IEnumerable<CryptoResult> results);
    }
}
