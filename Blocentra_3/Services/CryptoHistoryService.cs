using Blocentra_3.Models;
using Newtonsoft.Json;
using System.IO;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Service responsible for persisting and retrieving historical cryptocurrency data to/from a local JSON file.
    /// </summary>
    public class CryptoHistoryService : ICryptoHistoryService
    {
        // Path to the JSON file where historical data is stored
        private const string HistoryFilePath = "crypto_history.json";

        /// <summary>
        /// Loads historical cryptocurrency data from the local JSON file.
        /// Returns an empty list if the file does not exist or is empty.
        /// </summary>
        /// <returns>List of CryptoData objects representing historical records.</returns>
        public List<CryptoData> LoadHistory()
        {
            // If the file does not exist, return an empty list
            if (!File.Exists(HistoryFilePath))
                return new List<CryptoData>();

            // Read file content and deserialize it into a list of CryptoData
            // If deserialization fails or returns null, return an empty list
            return JsonConvert.DeserializeObject<List<CryptoData>>(File.ReadAllText(HistoryFilePath))
                   ?? new List<CryptoData>();
        }

        /// <summary>
        /// Saves the provided list of historical cryptocurrency data to a local JSON file.
        /// </summary>
        /// <param name="history">List of CryptoData objects to persist.</param>
        public void SaveHistory(List<CryptoData> history)
        {
            // Serialize the list to JSON and write to file
            File.WriteAllText(HistoryFilePath, JsonConvert.SerializeObject(history));
        }
    }
}
