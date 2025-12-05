using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECGViewer.Wpf.Services
{
    /// <summary>
    /// Interface for file dialog operations
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// Shows an open file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="filter">File filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <param name="multiSelect">Allow multiple file selection</param>
        /// <returns>The selected file path, or null if cancelled</returns>
        Task<string> ShowOpenFileDialogAsync(string title = "Open File", string filter = "All files (*.*)|*.*", string initialDirectory = "", bool multiSelect = false);

        /// <summary>
        /// Shows an open file dialog that allows multiple file selection
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="filter">File filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <returns>The selected file paths, or empty list if cancelled</returns>
        Task<IEnumerable<string>> ShowOpenFileDialogMultipleAsync(string title = "Open Files", string filter = "All files (*.*)|*.*", string initialDirectory = "");

        /// <summary>
        /// Shows a save file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="filter">File filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <param name="defaultFileName">Default file name</param>
        /// <returns>The selected file path, or null if cancelled</returns>
        Task<string> ShowSaveFileDialogAsync(string title = "Save File", string filter = "All files (*.*)|*.*", string initialDirectory = "", string defaultFileName = "");

        /// <summary>
        /// Shows a folder selection dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <returns>The selected folder path, or null if cancelled</returns>
        Task<string> ShowFolderDialogAsync(string title = "Select Folder", string initialDirectory = "");
    }
}