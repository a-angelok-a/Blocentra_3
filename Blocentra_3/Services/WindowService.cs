
using System.Windows;

namespace Blocentra_3.Services
{
    public class WindowService : IWindowService
    {
        private readonly Window _window;
        public WindowService(Window window)
        {
            _window = window;
        }
        public void CloseWindowCommand()
        {
            _window.Close();
        }

        public void MaximizeRestoreCommand()
        {
            if (_window.WindowState == WindowState.Maximized)
                _window.WindowState = WindowState.Normal;
            else
                _window.WindowState = WindowState.Maximized;
        }

        public void MinimizeRestoreCommand()
        {
            _window.WindowState = WindowState.Minimized;
        }

    }
}
