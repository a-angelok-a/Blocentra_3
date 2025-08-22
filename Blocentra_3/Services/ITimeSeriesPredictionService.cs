using Blocentra_3.Models;
using System.Collections.Generic;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Defines a contract for time series prediction of cryptocurrency prices.
    /// Implementations should support training on historical data and forecasting future prices.
    /// </summary>
    public interface ITimeSeriesPredictionService
    {
        /// <summary>
        /// Trains the prediction model using historical cryptocurrency data.
        /// </summary>
        /// <param name="historicalData">A collection of <see cref="CryptoData"/> representing past price points.</param>
        /// <param name="horizon">The number of future time steps the model should be trained to predict.</param>
        void TrainModel(IEnumerable<CryptoData> historicalData, int horizon);

        /// <summary>
        /// Generates a forecast of cryptocurrency prices for a specified horizon.
        /// </summary>
        /// <param name="horizon">The number of future time steps to forecast.</param>
        /// <returns>
        /// An array of <see cref="float"/> values representing the predicted prices for each future time step.
        /// The length of the array should match the specified horizon.
        /// </returns>
        float[] Forecast(int horizon);
    }
}
