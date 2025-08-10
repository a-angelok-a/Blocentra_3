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

        private string _symbol = "btc";
        public string Symbol
        {
            get => _symbol;
            set => SetProperty(ref _symbol, value);
        }

        public ObservableCollection<string> PricesFromExchanges { get; } = new();

        public ObservableCollection<string> Currencies { get; } = new()
        {
             "btc",
             "eth",
             "usdt",
             "bnb",
             "ada"
        };
        public MainViewModel(IRegionManager regionManager, IEnumerable<ICryptoApiService> apiService)
        {
            _apiService = new List<ICryptoApiService>(apiService);
            _regionManager = regionManager;

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

            foreach (var service in _apiService)
            {
                var result = await service.GetCurrencyAsync(Symbol);
                if (result.IsSuccess)
                {
                    PricesFromExchanges.Add($"{result.Currency.Symbol}: {result.Currency.PriceUsd} USD ({result.Currency.ExchangeName})");
                }
                else
                {
                    PricesFromExchanges.Add($"Ошибка {service.ExchangeName}: {result.ErrorMessage}");
                }
            }
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
