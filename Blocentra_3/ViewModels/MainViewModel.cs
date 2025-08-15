using Blocentra_3.Models;
using Blocentra_3.Services;
using Blocentra_3.Views;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Blocentra_3.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly ICryptoHistoryService _historyService;
        private readonly ITimeSeriesPredictionService _predictionService;
        private readonly List<ICryptoApiService> _apiService;
        private readonly IRegionManager _regionManager;
        private readonly IPriceAnalysisService _priceAnalysisService;

        public ObservableCollection<float> DailyForecast { get; } = new ObservableCollection<float>();
        public ObservableCollection<float> MonthlyForecast { get; } = new ObservableCollection<float>();

        private string _symbol = "btc";
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

        public ObservableCollection<CryptoCurrency> PricesFromExchanges { get; } = new();

        private CryptoCurrency _lowestPriceCurrency;
        public CryptoCurrency LowestPriceCurrency
        {
            get => _lowestPriceCurrency;
            set => SetProperty(ref _lowestPriceCurrency, value);
        }

        private CryptoCurrency _highestPriceCurrency;
        public CryptoCurrency HighestPriceCurrency
        {
            get => _highestPriceCurrency;
            set => SetProperty(ref _highestPriceCurrency, value);
        }

        private List<CryptoData> _historicalData = new();
        private List<CryptoData> _predictedData = new();

        public ObservableCollection<string> Currencies { get; } = new()
        {
             "btc",
             "eth",
             "usdt",
             "bnb",
             "ada"
        };
        public MainViewModel(IRegionManager regionManager, IEnumerable<ICryptoApiService> apiService, IPriceAnalysisService priceAnalysisService, ITimeSeriesPredictionService predictionService, ICryptoHistoryService historyService)
        {
            _apiService = new List<ICryptoApiService>(apiService);
            _regionManager = regionManager;
            _priceAnalysisService = priceAnalysisService;
            _predictionService = predictionService;
            _historyService = historyService;

            _historicalData = _historyService.LoadHistory();

            InitializeApp();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(60)
            };
            timer.Tick += async (s, e) => await LoadCurrencyAsync();
            timer.Start();
        }
        public async Task LoadCurrencyAsync()
        {
            PricesFromExchanges.Clear();

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
                    PricesFromExchanges.Add(new CryptoCurrency
                    {
                        Symbol = "Ошибка",
                        BidPrice = 0m,
                        AskPrice = 0m,
                        ExchangeName = result.Currency?.ExchangeName ?? "Unknown"
                    });
                }
            }

            _historyService.SaveHistory(_historicalData);

            await UpdateForecastsAsync();

            LowestPriceCurrency = _priceAnalysisService.GetLowerPrice(results);
            HighestPriceCurrency = _priceAnalysisService.GetHighestPrice(results);
        }

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

                var dailyForecast = _predictionService.Forecast(1);
                var monthlyForecast = _predictionService.Forecast(1);

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