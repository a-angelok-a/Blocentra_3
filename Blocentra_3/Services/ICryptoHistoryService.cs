using Blocentra_3.Models;
using System.Collections.Generic;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Defines a contract for services that manage the persistence of cryptocurrency historical data.
    /// Implementations are responsible for loading and saving historical crypto data to a storage medium (e.g., file, database).
    /// </summary>
    public interface ICryptoHistoryService
    {
        /// <summary>
        /// Loads the saved cryptocurrency history from persistent storage.
        /// </summary>
        /// <returns>
        /// A list of <see cref="CryptoData"/> representing previously stored historical data.
        /// Returns an empty list if no history exists.
        /// </returns>
        List<CryptoData> LoadHistory();

        /// <summary>
        /// Saves the provided cryptocurrency history to persistent storage.
        /// </summary>
        /// <param name="history">A list of <see cref="CryptoData"/> to be saved.</param>
        void SaveHistory(List<CryptoData> history);
    }
}
