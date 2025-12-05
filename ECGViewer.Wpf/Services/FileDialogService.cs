using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ECGViewer.Wpf.Services
{
    /// <summary>
    /// WPF implementation of IFileDialogService
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        /// <summary>
        /// Shows an open file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="filter">File filter</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <param name="multiSelect">Allow multiple file selection</param>
        /// <returns>The selected file path, or null if cancelled</returns>
        public Task<string> ShowOpenFileDialogAsync(string title = "Open File", string filter = "All files (*.*)|*.*", string initialDirectory = "", bool multiSelect = false)
        {
            return Task.Run(() =>
            {
                var dialog = new OpenFileDialog
                {
                    Title = title,
                    Filter = filter,
                    InitialDirectory = initialDirectory,
                    Multiselect = multiSelect
                };

                if (dialog.ShowDialog() == true)
                {
                    return dialog.FileName;
                }

                return null;
            });
        }

        /// <summary>
        /// Shows an open file dialog that allows multiple file selection
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="filter">File filter</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <returns>The selected file paths, or empty list if cancelled</returns>
        public Task<IEnumerable<string>> ShowOpenFileDialogMultipleAsync(string title = "Open Files", string filter = "All files (*.*)|*.*", string initialDirectory = "")
        {
            return Task.Run(() =>
            {
                var dialog = new OpenFileDialog
                {
                    Title = title,
                    Filter = filter,
                    InitialDirectory = initialDirectory,
                    Multiselect = true
                };

                if (dialog.ShowDialog() == true)
                {
                    return dialog.FileNames;
                }

                return new List<string>();
            });
        }

        /// <summary>
        /// Shows a save file dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="filter">File filter</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <param name="defaultFileName">Default file name</param>
        /// <returns>The selected file path, or null if cancelled</returns>
        public Task<string> ShowSaveFileDialogAsync(string title = "Save File", string filter = "All files (*.*)|*.*", string initialDirectory = "", string defaultFileName = "")
        {
            return Task.Run(() =>
            {
                var dialog = new SaveFileDialog
                {
                    Title = title,
                    Filter = filter,
                    InitialDirectory = initialDirectory,
                    FileName = defaultFileName
                };

                if (dialog.ShowDialog() == true)
                {
                    return dialog.FileName;
                }

                return null;
            });
        }

        /// <summary>
        /// Shows a folder selection dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="initialDirectory">Initial directory</param>
        /// <returns>The selected folder path, or null if cancelled</returns>
        public Task<string> ShowFolderDialogAsync(string title = "Select Folder", string initialDirectory = "")
        {
            return Task.Run(() =>
            {
                // For folder selection, we need to use a different approach since WPF doesn't have a built-in folder dialog
                // We'll use the System.Windows.Forms.FolderBrowserDialog
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    dialog.Description = title;
                    dialog.SelectedPath = initialDirectory;
                    dialog.ShowNewFolderButton = true;

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        return dialog.SelectedPath;
                    }

                    return null;
                }
            });
        }
    }
}