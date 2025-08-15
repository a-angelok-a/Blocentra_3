using Blocentra_3.Models;

namespace Blocentra_3.Services
{
    public interface ITimeSeriesPredictionService
    {
        void TrainModel(IEnumerable<CryptoData> historicalData, int horizon);
        float[] Forecast(int horizon);
    }
}