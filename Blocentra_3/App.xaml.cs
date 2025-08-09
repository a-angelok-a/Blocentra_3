using Blocentra_3.Services;
using Blocentra_3.ViewModels;
using Blocentra_3.Views;
using System.Net.Http;
using System.Windows;

namespace Blocentra_3
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var window = Container.Resolve<MainView>();

            Container.GetContainer().RegisterInstance<IWindowService>(new WindowService(window));

            return window;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<IWindowService, WindowService>();
            containerRegistry.Register<ICryptoApiService, CoinGeckoApiService>();

            containerRegistry.RegisterInstance(new HttpClient());
            containerRegistry.RegisterForNavigation<SplashScreenView>();
            containerRegistry.RegisterForNavigation<HeaderView>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager = Container.Resolve<IRegionManager>();
         
            regionManager.RequestNavigate("SplashScreenRegion", nameof(SplashScreenView));
        }
    }

}
