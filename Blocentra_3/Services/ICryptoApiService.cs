using Blocentra_3.Models;

namespace Blocentra_3.Services
{
    public interface ICryptoApiService
    {
        string ExchangeName { get; }
        Task<CryptoResult> GetCurrencyAsync(string symbol);
    }
}
