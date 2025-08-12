using Blocentra_3.Models;
using Newtonsoft.Json;
using System.IO;

namespace Blocentra_3.Services
{
    public class CryptoHistoryService : ICryptoHistoryService
    {
        private const string HistoryFilePath = "crypto_history.json";

        public List<CryptoData> LoadHistory()
        {
            if (!File.Exists(HistoryFilePath))
                return new List<CryptoData>();

            return JsonConvert.DeserializeObject<List<CryptoData>>(File.ReadAllText(HistoryFilePath))
                   ?? new List<CryptoData>();
        }

        public void SaveHistory(List<CryptoData> history)
        {
            File.WriteAllText(HistoryFilePath, JsonConvert.SerializeObject(history));
        }
    }
}
