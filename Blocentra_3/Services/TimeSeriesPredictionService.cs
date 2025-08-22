using Blocentra_3.Models;
using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Service for time series forecasting of cryptocurrency prices using ML.NET SSA (Singular Spectrum Analysis) model.
    /// Implements <see cref="ITimeSeriesPredictionService"/>.
    /// </summary>
    public class TimeSeriesPredictionService : ITimeSeriesPredictionService
    {
        private readonly MLContext _mlContext;
        private TimeSeriesPredictionEngine<CryptoData, ForecastOutput> _forecastEngine;
        private List<CryptoData> _trainingData;

        /// <summary>
        /// Initializes a new instance of <see cref="TimeSeriesPredictionService"/> with a fixed random seed for reproducibility.
        /// </summary>
        public TimeSeriesPredictionService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        /// <summary>
        /// Trains the SSA time series model on historical cryptocurrency data.
        /// </summary>
        /// <param name="historicalData">Historical price data.</param>
        /// <param name="horizon">Number of future periods to forecast.</param>
        public void TrainModel(IEnumerable<CryptoData> historicalData, int horizon)
        {
            if (historicalData == null || !historicalData.Any())
                throw new ArgumentException("Historical data cannot be empty");

            // Sort data by timestamp to ensure chronological order
            _trainingData = historicalData.OrderBy(d => d.Timestamp).ToList();

            var trainingDataView = _mlContext.Data.LoadFromEnumerable(_trainingData);

            // Define SSA forecasting pipeline
            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(ForecastOutput.ForecastedPrice),
                inputColumnName: nameof(CryptoData.Price),
                windowSize: Math.Min(7, _trainingData.Count), // sliding window size
                seriesLength: _trainingData.Count,            // length of the series
                trainSize: _trainingData.Count,               // number of points to train on
                horizon: horizon,                             // forecast horizon
                confidenceLevel: 0.95f,                       // 95% confidence intervals
                confidenceLowerBoundColumn: nameof(ForecastOutput.LowerBoundPrice),
                confidenceUpperBoundColumn: nameof(ForecastOutput.UpperBoundPrice));

            // Fit the model on training data
            var model = forecastingPipeline.Fit(trainingDataView);

            // Create a prediction engine for real-time forecasting
            _forecastEngine = model.CreateTimeSeriesEngine<CryptoData, ForecastOutput>(_mlContext);
        }

        /// <summary>
        /// Predicts future prices for the given horizon using the trained SSA model.
        /// </summary>
        /// <param name="horizon">Number of future periods to forecast.</param>
        /// <returns>Array of predicted prices.</returns>
        public float[] Forecast(int horizon)
        {
            if (_trainingData == null || !_trainingData.Any())
                throw new InvalidOperationException("Training data is empty");

            // Make initial forecast
            var forecast = _forecastEngine.Predict();
            var results = new List<float>(forecast.ForecastedPrice);

            // Handle longer horizons than initial forecast
            if (horizon > forecast.ForecastedPrice.Length)
            {
                var extraHorizon = horizon - forecast.ForecastedPrice.Length;

                // Re-train the engine with extended horizon
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

            // Return only requested number of predictions
            return results.Take(horizon).ToArray();
        }

        /// <summary>
        /// Internal class to hold SSA forecast outputs and confidence bounds.
        /// </summary>
        private class ForecastOutput
        {
            public float[] ForecastedPrice { get; set; }
            public float[] LowerBoundPrice { get; set; }
            public float[] UpperBoundPrice { get; set; }
        }
    }
}
