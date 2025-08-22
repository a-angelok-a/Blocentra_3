namespace Blocentra_3.Models
{
    /// <summary>
    /// Represents the result of an operation that fetches or processes a cryptocurrency.
    /// Provides success/failure status, error message, and the resulting <see cref="CryptoCurrency"/> object.
    /// </summary>
    public class CryptoResult
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Contains an error message if the operation failed; otherwise, null.
        /// </summary>
        public string ErrorMessage { get; init; }

        /// <summary>
        /// The resulting <see cref="CryptoCurrency"/> if the operation was successful; otherwise, null.
        /// </summary>
        public CryptoCurrency Currency { get; init; }

        /// <summary>
        /// Creates a successful result containing the provided <see cref="CryptoCurrency"/>.
        /// </summary>
        /// <param name="currency">The cryptocurrency object returned by the operation.</param>
        /// <returns>A <see cref="CryptoResult"/> indicating success.</returns>
        public static CryptoResult Ok(CryptoCurrency currency)
        {
            return new CryptoResult
            {
                IsSuccess = true,
                Currency = currency
            };
        }

        /// <summary>
        /// Creates a failed result with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message describing why the operation failed.</param>
        /// <returns>A <see cref="CryptoResult"/> indicating failure.</returns>
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
