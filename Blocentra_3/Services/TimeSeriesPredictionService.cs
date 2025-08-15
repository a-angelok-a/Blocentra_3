using Blocentra_3.Models;
using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;

namespace Blocentra_3.Services
{
    public class TimeSeriesPredictionService : ITimeSeriesPredictionService
    {
        private readonly MLContext _mlContext;
        private TimeSeriesPredictionEngine<CryptoData, ForecastOutput> _forecastEngine;
        private List<CryptoData> _trainingData;

        public TimeSeriesPredictionService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<CryptoData> historicalData, int horizon)
        {
            if (historicalData == null || !historicalData.Any())
                throw new ArgumentException("Historical data cannot be empty");

            _trainingData = historicalData.OrderBy(d => d.Timestamp).ToList();

            var trainingDataView = _mlContext.Data.LoadFromEnumerable(_trainingData);

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(ForecastOutput.ForecastedPrice),
                inputColumnName: nameof(CryptoData.Price),
                windowSize: Math.Min(7, _trainingData.Count), 
                seriesLength: _trainingData.Count,
                trainSize: _trainingData.Count,
                horizon: horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(ForecastOutput.LowerBoundPrice),
                confidenceUpperBoundColumn: nameof(ForecastOutput.UpperBoundPrice));

            var model = forecastingPipeline.Fit(trainingDataView);

            _forecastEngine = model.CreateTimeSeriesEngine<CryptoData, ForecastOutput>(_mlContext);
        }

        public float[] Forecast(int horizon)
        {
            if (_forecastEngine == null)
                throw new InvalidOperationException("Model has not been trained.");

            var forecast = _forecastEngine.Predict();

            var results = new List<float>(forecast.ForecastedPrice);

            if (horizon > forecast.ForecastedPrice.Length)
            {
                var extraHorizon = horizon - forecast.ForecastedPrice.Length;
                _forecastEngine = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: nameof(ForecastOutput.ForecastedPrice),
                    inputColumnName: nameof(CryptoData.Price),
                    windowSize: Math.Min(7, _trainingData.Count),
                    seriesLength: _trainingData.Count,
                    trainSize: _trainingData.Count,
                    horizon: extraHorizon)
                    .Fit(_mlContext.Data.LoadFromEnumerable(_trainingData))
                    .CreateTimeSeriesEngine<CryptoData, ForecastOutput>(_mlContext);

                var extraForecast = _forecastEngine.Predict();
                results.AddRange(extraForecast.ForecastedPrice);
            }

            return results.Take(horizon).ToArray();
        }

        private class ForecastOutput
        {
            public float[] ForecastedPrice { get; set; }
            public float[] LowerBoundPrice { get; set; }
            public float[] UpperBoundPrice { get; set; }
        }
    }
}
