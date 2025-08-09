using Blocentra_3.Services;
using Blocentra_3.Views;
using System.Collections.ObjectModel;
using System.Printing;

namespace Blocentra_3.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly ICryptoApiService _apiService;
        private readonly IRegionManager _regionManager;

        private string _symbol = "btc";
        public string Symbol
        {
            get => _symbol;
            set => SetProperty(ref _symbol, value);
        }

        private string _priceInfo;
        public string PriceInfo
        {
            get => _priceInfo;
            set => SetProperty(ref _priceInfo, value);
        }

        public ObservableCollection<string> Currencies { get; } = new()
        {
            "bitcoin",
            "ethereum",
            "ripple",
            "litecoin",
            "cardano",
            "dogecoin",
            "polkadot",
            "stellar",
            "chainlink",
            "binancecoin"
        };
        public MainViewModel(IRegionManager regionManager, ICryptoApiService apiService)
        {
            _apiService = apiService;
            _regionManager = regionManager;
            InitializeApp();
        }
        public async Task LoadCurrencyAsync()
        {
            var result = await _apiService.GetCurrencyAsync(Symbol);

            PriceInfo = result.IsSuccess
                ? $"{result.Currency.Symbol}: {result.Currency.PriceUsd} USD ({result.Currency.ExchangeName})"
                : $"Ошибка: {result.ErrorMessage}";
        }

        private async void InitializeApp()
        {
            await Task.Delay(3000);

            var splashScreenRegion = _regionManager.Regions["SplashScreenRegion"];
            splashScreenRegion.RemoveAll();


            _regionManager.RequestNavigate("HeaderRegion", nameof(HeaderView));
            await LoadCurrencyAsync();
            //_regionManager.RequestNavigate("MainRegion", nameof(MainView));
        }
    }
}
