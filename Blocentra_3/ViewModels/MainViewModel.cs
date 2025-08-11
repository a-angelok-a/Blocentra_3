using Blocentra_3.Models;
using Blocentra_3.Services;
using Blocentra_3.Views;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Blocentra_3.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly List<ICryptoApiService> _apiService;
        private readonly IRegionManager _regionManager;
        private readonly IPriceAnalysisService _priceAnalysisService;

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

        public ObservableCollection<string> PricesFromExchanges { get; } = new();

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

        public ObservableCollection<string> Currencies { get; } = new()
        {
             "btc",
             "eth",
             "usdt",
             "bnb",
             "ada"
        };
        public MainViewModel(IRegionManager regionManager, IEnumerable<ICryptoApiService> apiService, IPriceAnalysisService priceAnalysisService)
        {
            _apiService = new List<ICryptoApiService>(apiService);
            _regionManager = regionManager;
            _priceAnalysisService = priceAnalysisService;

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
                    PricesFromExchanges.Add($"{result.Currency.Symbol}: Bid = {result.Currency.BidPrice} USD, Ask = {result.Currency.AskPrice} USD ({result.Currency.ExchangeName})");
                }
                else
                {
                    PricesFromExchanges.Add($"Ошибка {result.Currency?.ExchangeName ?? "Unknown"}: {result.ErrorMessage}");
                }
            }

            LowestPriceCurrency = _priceAnalysisService.GetLowerPrice(results);
            HighestPriceCurrency = _priceAnalysisService.GetHighestPrice(results);
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
