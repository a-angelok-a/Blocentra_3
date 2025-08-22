using Blocentra_3.Services;

namespace Blocentra_3.ViewModels
{
    /// <summary>
    /// ViewModel for the window header, providing commands for window control.
    /// Supports Close, Maximize/Restore, and Minimize actions via <see cref="IWindowService"/>.
    /// </summary>
    public class HeaderViewModel : BindableBase
    {
        /// <summary>
        /// Service responsible for window operations such as close, maximize, and minimize.
        /// </summary>
        private readonly IWindowService _windowService;

        /// <summary>
        /// Command to close the current window.
        /// </summary>
        public DelegateCommand CloseCommand { get; }

        /// <summary>
        /// Command to maximize or restore the current window.
        /// </summary>
        public DelegateCommand MaximizeCommand { get; }

        /// <summary>
        /// Command to minimize the current window.
        /// </summary>
        public DelegateCommand MinimizeCommand { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderViewModel"/>.
        /// </summary>
        /// <param name="windowService">
        /// The <see cref="IWindowService"/> used to perform window operations.
        /// </param>
        public HeaderViewModel(IWindowService windowService)
        {
            _windowService = windowService;

            // Initialize commands with actions that delegate to the window service.
            CloseCommand = new DelegateCommand(() => _windowService.CloseWindowCommand());
            MaximizeCommand = new DelegateCommand(() => _windowService.MaximizeRestoreCommand());
            MinimizeCommand = new DelegateCommand(() => _windowService.MinimizeRestoreCommand());
        }
    }
}
