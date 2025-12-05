using System;
using System.Threading.Tasks;
using System.Windows;

namespace ECGViewer.Wpf.Services
{
    /// <summary>
    /// WPF implementation of IDialogService
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Shows a message dialog with the specified message and title
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">The dialog title</param>
        /// <param name="button">The buttons to display</param>
        /// <param name="icon">The icon to display</param>
        /// <returns>The result of the dialog</returns>
        public Task<DialogResult> ShowMessageAsync(string message, string title = "", DialogButton button = DialogButton.OK, DialogIcon icon = DialogIcon.Information)
        {
            return Task.Run(() =>
            {
                var messageBoxButton = ConvertToMessageBoxButton(button);
                var messageBoxImage = ConvertToMessageBoxImage(icon);
                var result = MessageBox.Show(message, title, messageBoxButton, messageBoxImage);
                return ConvertFromMessageBoxResult(result);
            });
        }

        /// <summary>
        /// Shows a confirmation dialog
        /// </summary>
        /// <param name="message">The confirmation message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>True if the user confirmed, false otherwise</returns>
        public async Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
        {
            var result = await ShowMessageAsync(message, title, DialogButton.YesNo, DialogIcon.Question);
            return result == DialogResult.Yes;
        }

        /// <summary>
        /// Shows an error dialog
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>Task representing the async operation</returns>
        public Task ShowErrorAsync(string message, string title = "Error")
        {
            return ShowMessageAsync(message, title, DialogButton.OK, DialogIcon.Error);
        }

        /// <summary>
        /// Shows a warning dialog
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>Task representing the async operation</returns>
        public Task ShowWarningAsync(string message, string title = "Warning")
        {
            return ShowMessageAsync(message, title, DialogButton.OK, DialogIcon.Warning);
        }

        /// <summary>
        /// Shows an information dialog
        /// </summary>
        /// <param name="message">The information message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>Task representing the async operation</returns>
        public Task ShowInformationAsync(string message, string title = "Information")
        {
            return ShowMessageAsync(message, title, DialogButton.OK, DialogIcon.Information);
        }

        private MessageBoxButton ConvertToMessageBoxButton(DialogButton button)
        {
            return button switch
            {
                DialogButton.OK => MessageBoxButton.OK,
                DialogButton.OKCancel => MessageBoxButton.OKCancel,
                DialogButton.YesNo => MessageBoxButton.YesNo,
                DialogButton.YesNoCancel => MessageBoxButton.YesNoCancel,
                _ => MessageBoxButton.OK
            };
        }

        private MessageBoxImage ConvertToMessageBoxImage(DialogIcon icon)
        {
            return icon switch
            {
                DialogIcon.Information => MessageBoxImage.Information,
                DialogIcon.Warning => MessageBoxImage.Warning,
                DialogIcon.Error => MessageBoxImage.Error,
                DialogIcon.Question => MessageBoxImage.Question,
                DialogIcon.None => MessageBoxImage.None,
                _ => MessageBoxImage.Information
            };
        }

        private DialogResult ConvertFromMessageBoxResult(MessageBoxResult result)
        {
            return result switch
            {
                MessageBoxResult.OK => DialogResult.OK,
                MessageBoxResult.Cancel => DialogResult.Cancel,
                MessageBoxResult.Yes => DialogResult.Yes,
                MessageBoxResult.No => DialogResult.No,
                _ => DialogResult.None
            };
        }
    }
}