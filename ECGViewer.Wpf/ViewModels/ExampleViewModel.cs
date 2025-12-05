using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ECGViewer.Wpf.Commands;
using ECGViewer.Wpf.Services;
using ECGViewer.Wpf.Timing;

namespace ECGViewer.Wpf.ViewModels
{
    /// <summary>
    /// Example view model demonstrating the MVVM infrastructure usage
    /// </summary>
    public class ExampleViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IFileDialogService _fileDialogService;
        private readonly IDispatcherTimer _timer;
        private int _counter;
        private string _statusMessage;
        private bool _isProcessing;

        public ExampleViewModel(
            IDialogService dialogService,
            IFileDialogService fileDialogService,
            IDispatcherTimerFactory timerFactory)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            _timer = timerFactory.CreateTimer();

            // Initialize commands
            ShowMessageCommand = new RelayCommand(ShowMessage, CanShowMessage);
            OpenFileCommand = new AsyncRelayCommand(OpenFileAsync, CanOpenFile);
            StartTimerCommand = new RelayCommand(StartTimer, CanStartTimer);
            StopTimerCommand = new RelayCommand(StopTimer, CanStopTimer);

            // Setup timer
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;

            StatusMessage = "Ready";
        }

        #region Properties

        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        #endregion

        #region Commands

        public RelayCommand ShowMessageCommand { get; }
        public AsyncRelayCommand OpenFileCommand { get; }
        public RelayCommand StartTimerCommand { get; }
        public RelayCommand StopTimerCommand { get; }

        #endregion

        #region Command Implementations

        private bool CanShowMessage()
        {
            return !IsProcessing;
        }

        private void ShowMessage()
        {
            _dialogService.ShowInformationAsync($"Current counter value: {Counter}", "Information");
        }

        private bool CanOpenFile()
        {
            return !IsProcessing;
        }

        private async Task OpenFileAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Opening file...";

                var filePath = await _fileDialogService.ShowOpenFileDialogAsync(
                    "Select a TXT file",
                    "Text files (*.txt)|*.txt|All files (*.*)|*.*");

                if (!string.IsNullOrEmpty(filePath))
                {
                    StatusMessage = $"File selected: {System.IO.Path.GetFileName(filePath)}";
                    await _dialogService.ShowInformationAsync($"Successfully loaded: {filePath}", "File Loaded");
                }
                else
                {
                    StatusMessage = "File selection cancelled";
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync($"Error opening file: {ex.Message}", "Error");
                StatusMessage = "Error occurred";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private bool CanStartTimer()
        {
            return !_timer.IsEnabled && !IsProcessing;
        }

        private void StartTimer()
        {
            _timer.Start();
            StatusMessage = "Timer started";
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanStopTimer()
        {
            return _timer.IsEnabled;
        }

        private void StopTimer()
        {
            _timer.Stop();
            StatusMessage = "Timer stopped";
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region Timer Event Handler

        private void OnTimerTick(object sender, EventArgs e)
        {
            Counter++;
            StatusMessage = $"Timer tick: {Counter}";

            // Raise command changed events to update CanExecute
            ShowMessageCommand.RaiseCanExecuteChanged();
            StartTimerCommand.RaiseCanExecuteChanged();
            StopTimerCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Validation Example

        protected override void ValidateProperty(string propertyName, object value)
        {
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(StatusMessage):
                    if (value is string message && string.IsNullOrWhiteSpace(message))
                    {
                        AddError(propertyName, "Status message cannot be empty");
                    }
                    break;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}