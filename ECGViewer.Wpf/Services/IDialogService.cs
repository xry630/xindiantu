using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECGViewer.Wpf.Services
{
    /// <summary>
    /// Interface for showing dialog messages to the user
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a message dialog with the specified message and title
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">The dialog title</param>
        /// <param name="button">The buttons to display</param>
        /// <param name="icon">The icon to display</param>
        /// <returns>The result of the dialog</returns>
        Task<DialogResult> ShowMessageAsync(string message, string title = "", DialogButton button = DialogButton.OK, DialogIcon icon = DialogIcon.Information);

        /// <summary>
        /// Shows a confirmation dialog
        /// </summary>
        /// <param name="message">The confirmation message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>True if the user confirmed, false otherwise</returns>
        Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation");

        /// <summary>
        /// Shows an error dialog
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>Task representing the async operation</returns>
        Task ShowErrorAsync(string message, string title = "Error");

        /// <summary>
        /// Shows a warning dialog
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>Task representing the async operation</returns>
        Task ShowWarningAsync(string message, string title = "Warning");

        /// <summary>
        /// Shows an information dialog
        /// </summary>
        /// <param name="message">The information message</param>
        /// <param name="title">The dialog title</param>
        /// <returns>Task representing the async operation</returns>
        Task ShowInformationAsync(string message, string title = "Information");
    }

    /// <summary>
    /// Dialog button options
    /// </summary>
    public enum DialogButton
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    /// <summary>
    /// Dialog icon options
    /// </summary>
    public enum DialogIcon
    {
        None,
        Information,
        Warning,
        Error,
        Question
    }

    /// <summary>
    /// Dialog result options
    /// </summary>
    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }
}