using Blocentra_3.Models;

namespace Blocentra_3.Services
{
    public interface ICryptoHistoryService
    {
        List<CryptoData> LoadHistory();
        void SaveHistory(List<CryptoData> history);
    }
}
