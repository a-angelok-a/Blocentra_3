
namespace Blocentra_3.Models
{
    public class CryptoResult
    {
        public bool IsSuccess { get; init; }
        public string ErrorMessage { get; init; }
        public CryptoCurrency Currency { get; init; }

        public static CryptoResult Ok(CryptoCurrency currency)
        {
            return new CryptoResult
            {
                IsSuccess = true,
                Currency = currency
            };
        }

        public static CryptoResult Fail(string errorMessage)
        {
            return new CryptoResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
