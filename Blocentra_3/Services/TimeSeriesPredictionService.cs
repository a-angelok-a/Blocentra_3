using Blocentra_3.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Blocentra_3.Services
{
    public class TimeSeriesPredictionService : ITimeSeriesPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private List<CryptoDataWithIndex> _trainingData;

        public TimeSeriesPredictionService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<CryptoData> historicalData, int horizon)
        {
            if (historicalData == null || !historicalData.Any())
                throw new ArgumentException("Historical data cannot be empty");

            _trainingData = PrepareDataWithIndex(historicalData);

            var trainingDataView = _mlContext.Data.LoadFromEnumerable(_trainingData);

            var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(CryptoDataWithIndex.Price))
                .Append(_mlContext.Transforms.Concatenate("Features", nameof(CryptoDataWithIndex.TimeIndex)))
                .Append(_mlContext.Regression.Trainers.FastTree());

            _model = pipeline.Fit(trainingDataView);
        }

        public float[] Forecast(int horizon)
        {
            if (_model == null)
                throw new InvalidOperationException("Model has not been trained.");

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<CryptoDataWithIndex, PricePrediction>(_model);

            float lastIndex = _trainingData.Max(d => d.TimeIndex);
            var results = new List<float>();

            for (int i = 1; i <= horizon; i++)
            {
                var input = new CryptoDataWithIndex { TimeIndex = lastIndex + i };
                var prediction = predictionEngine.Predict(input);
                results.Add(prediction.Score);
            }

            return results.ToArray();
        }

        private List<CryptoDataWithIndex> PrepareDataWithIndex(IEnumerable<CryptoData> historicalData)
        {
            var ordered = historicalData.OrderBy(d => d.Timestamp).ToList();
            var startTime = ordered.First().Timestamp;

            return ordered.Select(d => new CryptoDataWithIndex
            {
                TimeIndex = (float)(d.Timestamp - startTime).TotalMinutes,
                Price = d.Price
            }).ToList();
        }

        private class CryptoDataWithIndex
        {
            public float TimeIndex { get; set; }
            public float Price { get; set; }
        }

        private class PricePrediction
        {
            [ColumnName("Score")]
            public float Score { get; set; }
        }
    }
}
