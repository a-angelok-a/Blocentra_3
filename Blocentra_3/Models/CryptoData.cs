
namespace Blocentra_3.Models
{
    public class CryptoData 
    {
        public DateTime Timestamp { get; set; }
        public float Price { get; set; }

        public float Label => Price;
    }
}
