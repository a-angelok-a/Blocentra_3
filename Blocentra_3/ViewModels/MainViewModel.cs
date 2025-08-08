using Blocentra_3.Views;

namespace Blocentra_3.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public MainViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            InitializeApp();
        }

        private async void InitializeApp()
        {
            // Имитация задержки (например, 3 секунды)
            await Task.Delay(3000);

            // Убираем SplashScreen
            var splashScreenRegion = _regionManager.Regions["SplashScreenRegion"];
            splashScreenRegion.RemoveAll();


            // Показываем Header и MainView
            _regionManager.RequestNavigate("HeaderRegion", nameof(HeaderView));
            _regionManager.RequestNavigate("MainRegion", nameof(MainView));
        }
    }
}
