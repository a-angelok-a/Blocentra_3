using Blocentra_3.Models;
using Blocentra_3.Services;
using Blocentra_3.Views;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Blocentra_3.ViewModels
{
    /// <summary>
    /// Main ViewModel responsible for managing cryptocurrency data, 
    /// including fetching prices from multiple exchanges, maintaining 
    /// historical data, and generating price forecasts.
    /// </summary>
    public class MainViewModel : BindableBase
    {
        private readonly ICryptoHistoryService _historyService;
        private readonly ITimeSeriesPredictionService _predictionService;
        private readonly List<ICryptoApiService> _apiService;
        private readonly IRegionManager _regionManager;
        private readonly IPriceAnalysisService _priceAnalysisService;

        /// <summary>
        /// Forecast for the next day(s) based on historical and predicted data.
        /// </summary>
        public ObservableCollection<float> DailyForecast { get; } = new ObservableCollection<float>();

        /// <summary>
        /// Forecast for the next month based on historical and predicted data.
        /// </summary>
        public ObservableCollection<float> MonthlyForecast { get; } = new ObservableCollection<float>();

        private string _symbol = "btc";
        /// <summary>
        /// The cryptocurrency symbol to display and analyze (e.g., BTC, ETH).
        /// Changing the symbol triggers reloading of data from APIs.
        /// </summary>
        public string Symbol
        {
            get => _symbol;
            set
            {
                if (SetProperty(ref _symbol, value))
                {
                    _ = LoadCurrencyAsync();
                }
            }
        }

        /// <summary>
        /// Collection of prices retrieved from multiple exchanges.
        /// </summary>
        public ObservableCollection<CryptoCurrency> PricesFromExchanges { get; } = new();

        private CryptoCurrency _lowestPriceCurrency;
        /// <summary>
        /// The cryptocurrency with the lowest bid price among all exchanges.
        /// </summary>
        public CryptoCurrency LowestPriceCurrency
        {
            get => _lowestPriceCurrency;
            set => SetProperty(ref _lowestPriceCurrency, value);
        }

        private CryptoCurrency _highestPriceCurrency;
        /// <summary>
        /// The cryptocurrency with the highest bid price among all exchanges.
        /// </summary>
        public CryptoCurrency HighestPriceCurrency
        {
            get => _highestPriceCurrency;
            set => SetProperty(ref _highestPriceCurrency, value);
        }

        private List<CryptoData> _historicalData = new();
        private List<CryptoData> _predictedData = new();

        /// <summary>
        /// Predefined list of cryptocurrency symbols available for selection in the UI.
        /// </summary>
        public ObservableCollection<string> Currencies { get; } = new()
        {
             "btc",
             "eth",
             "usdt",
             "bnb",
             "ada"
        };

        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/>.
        /// Sets up services, loads historical data, and starts periodic refresh.
        /// </summary>
        public MainViewModel(
            IRegionManager regionManager,
            IEnumerable<ICryptoApiService> apiService,
            IPriceAnalysisService priceAnalysisService,
            ITimeSeriesPredictionService predictionService,
            ICryptoHistoryService historyService)
        {
            _apiService = new List<ICryptoApiService>(apiService);
            _regionManager = regionManager;
            _priceAnalysisService = priceAnalysisService;
            _predictionService = predictionService;
            _historyService = historyService;

            _historicalData = _historyService.LoadHistory();

            InitializeApp();

            // Setup a timer to refresh currency data every 60 seconds.
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(60)
            };
            timer.Tick += async (s, e) => await LoadCurrencyAsync();
            timer.Start();
        }

        /// <summary>
        /// Loads cryptocurrency data from all APIs and updates relevant properties.
        /// Updates historical data and forecasts as well.
        /// </summary>
        public async Task LoadCurrencyAsync()
        {
            PricesFromExchanges.Clear();

            // Fetch data from all API services in parallel.
            var tasks = _apiService.Select(service => service.GetCurrencyAsync(Symbol)).ToList();
            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    PricesFromExchanges.Add(result.Currency);

                    _historicalData.Add(new CryptoData
                    {
                        Timestamp = DateTime.UtcNow,
                        Price = (float)result.Currency.BidPrice
                    });
                }
                else
                {
                    // If API call fails, add a placeholder with error info.
                    PricesFromExchanges.Add(new CryptoCurrency
                    {
                        Symbol = "Error",
                        BidPrice = 0m,
                        AskPrice = 0m,
                        ExchangeName = result.Currency?.ExchangeName ?? "Unknown"
                    });
                }
            }

            _historyService.SaveHistory(_historicalData);

            await UpdateForecastsAsync();

            // Update lowest/highest price currencies for UI display.
            LowestPriceCurrency = _priceAnalysisService.GetLowerPrice(results);
            HighestPriceCurrency = _priceAnalysisService.GetHighestPrice(results);
        }

        /// <summary>
        /// Updates daily and monthly forecasts using historical and predicted data.
        /// Limits the number of data points to avoid performance issues.
        /// </summary>
        private async Task UpdateForecastsAsync()
        {
            const int MaxDataPoints = 90;

            _historicalData = _historicalData
                .OrderBy(d => d.Timestamp)
                .Skip(Math.Max(0, _historicalData.Count - MaxDataPoints))
                .ToList();

            _predictedData = _predictedData
                .OrderBy(d => d.Timestamp)
                .Skip(Math.Max(0, _predictedData.Count - MaxDataPoints))
                .ToList();

            var combinedData = _historicalData.Concat(_predictedData)
                                              .OrderBy(d => d.Timestamp)
                                              .ToList();

            if (!combinedData.Any())
                return;

            await Task.Run(() =>
            {
                _predictionService.TrainModel(combinedData, horizon: 30);

                var monthlyForecast = _predictionService.Forecast(30);
                var dailyForecast = new[] { monthlyForecast.First() };

                // Update UI collections on the main thread.
                App.Current.Dispatcher.Invoke(() =>
                {
                    DailyForecast.Clear();
                    foreach (var val in dailyForecast)
                        DailyForecast.Add(val);

                    MonthlyForecast.Clear();
                    foreach (var val in monthlyForecast)
                        MonthlyForecast.Add(val);
                });

                var lastTimestamp = combinedData.Max(d => d.Timestamp);
                _predictedData = monthlyForecast.Select((price, idx) => new CryptoData
                {
                    Timestamp = lastTimestamp.AddDays(idx + 1),
                    Price = price
                }).ToList();
            });
        }

        /// <summary>
        /// Initializes the application, removes the splash screen, and navigates to header view.
        /// Also triggers initial data loading after a brief delay.
        /// </summary>
        private async void InitializeApp()
        {
            await Task.Delay(3000);

            var splashScreenRegion = _regionManager.Regions["SplashScreenRegion"];
            splashScreenRegion.RemoveAll();

            _regionManager.RequestNavigate("HeaderRegion", nameof(HeaderView));
            await LoadCurrencyAsync();
        }
    }
}
