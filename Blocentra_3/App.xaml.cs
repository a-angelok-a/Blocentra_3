using Blocentra_3.Services;
using Blocentra_3.Views;
using System.Net.Http;
using System.Windows;

namespace Blocentra_3
{
    // Main application class inheriting from PrismApplication to leverage Prism framework features
    public partial class App : PrismApplication
    {
        // Creates the main window (shell) of the application
        protected override Window CreateShell()
        {
            // Resolve MainView from the Prism container
            var window = Container.Resolve<MainView>();

            // Register a singleton instance of IWindowService to manage window operations
            Container.GetContainer().RegisterInstance<IWindowService>(new WindowService(window));

            return window;
        }

        // Register application-wide services and dependencies in the Prism container
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register business services
            containerRegistry.Register<ICryptoHistoryService, CryptoHistoryService>();
            containerRegistry.Register<ITimeSeriesPredictionService, TimeSeriesPredictionService>();
            containerRegistry.Register<IPriceAnalysisService, PriceAnalysisService>();

            // Register external API services with unique names for multi-implementation resolution
            containerRegistry.Register<ICryptoApiService, BitfinexApiService>("Bitfinex");
            containerRegistry.Register<ICryptoApiService, CoinGeckoApiService>("CoinGecko");
            containerRegistry.Register<ICryptoApiService, BitstampApiService>("Bitstamp");
            containerRegistry.Register<ICryptoApiService, HuobiApiService>("Huobi");
            containerRegistry.Register<ICryptoApiService, OkexApiService>("Okex");

            // Register a shared HttpClient instance for API calls
            containerRegistry.RegisterInstance(new HttpClient());

            // Register views for navigation using Prism's RegionManager
            containerRegistry.RegisterForNavigation<SplashScreenView>();
            containerRegistry.RegisterForNavigation<HeaderView>();
        }

        // Called when the application is initialized
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Navigate to the splash screen view in the designated region
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("SplashScreenRegion", nameof(SplashScreenView));
        }
    }
}
