using Blocentra_3.Models;

namespace Blocentra_3.Services
{
    public interface IPriceAnalysisService
    {
        CryptoCurrency GetLowerPrice(IEnumerable<CryptoResult> results);
        CryptoCurrency GetHighestPrice(IEnumerable<CryptoResult> results);
    }
}
