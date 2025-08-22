using System.Windows;

namespace Blocentra_3.Services
{
    /// <summary>
    /// Provides window management operations such as close, maximize/restore, and minimize for a WPF window.
    /// Implements <see cref="IWindowService"/>.
    /// </summary>
    public class WindowService : IWindowService
    {
        private readonly Window _window;

        /// <summary>
        /// Initializes a new instance of <see cref="WindowService"/> for the specified <see cref="Window"/>.
        /// </summary>
        /// <param name="window">The WPF window to manage.</param>
        public WindowService(Window window)
        {
            _window = window;
        }

        /// <summary>
        /// Closes the associated window.
        /// </summary>
        public void CloseWindowCommand()
        {
            _window.Close();
        }

        /// <summary>
        /// Toggles the window state between maximized and normal.
        /// </summary>
        public void MaximizeRestoreCommand()
        {
            if (_window.WindowState == WindowState.Maximized)
                _window.WindowState = WindowState.Normal;
            else
                _window.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        public void MinimizeRestoreCommand()
        {
            _window.WindowState = WindowState.Minimized;
        }
    }
}
