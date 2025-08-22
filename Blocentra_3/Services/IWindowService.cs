namespace Blocentra_3.Services
{
    /// <summary>
    /// Defines a contract for window management operations.
    /// Implementations should provide methods to close, maximize/restore, and minimize windows.
    /// </summary>
    public interface IWindowService
    {
        /// <summary>
        /// Closes the associated window.
        /// </summary>
        void CloseWindowCommand();

        /// <summary>
        /// Toggles the associated window between maximized and restored states.
        /// </summary>
        void MaximizeRestoreCommand();

        /// <summary>
        /// Minimizes the associated window.
        /// </summary>
        void MinimizeRestoreCommand();
    }
}
