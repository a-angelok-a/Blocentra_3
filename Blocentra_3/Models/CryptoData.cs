namespace Blocentra_3.Models
{
    /// <summary>
    /// Represents a single data point in the historical or real-time 
    /// price feed of a cryptocurrency.
    /// </summary>
    public class CryptoData
    {
        /// <summary>
        /// The UTC timestamp indicating when this price snapshot was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The price of the cryptocurrency at the specified <see cref="Timestamp"/>.
        /// </summary>
        public float Price { get; set; }
    }
}
