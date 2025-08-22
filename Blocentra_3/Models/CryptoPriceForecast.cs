
namespace Blocentra_3.Models
{
    /// <summary>
    /// Represents the output of a price forecast or prediction model 
    /// for a cryptocurrency.
    /// </summary>
    public class CryptoPriceForecast
    {
        /// <summary>
        /// The confidence score or prediction value returned by the forecasting model.
        /// </summary>
        public float Score { get; set; }
    }
}
