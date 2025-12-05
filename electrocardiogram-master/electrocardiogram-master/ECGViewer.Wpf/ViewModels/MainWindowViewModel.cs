using CommunityToolkit.Mvvm.Input;

namespace ECGViewer.Wpf.ViewModels
{
    /// <summary>
    /// ViewModel for the MainWindow view.
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
        }

        [RelayCommand]
        private void LoadData()
        {
            // TODO: Implement data loading logic
        }
    }
}
