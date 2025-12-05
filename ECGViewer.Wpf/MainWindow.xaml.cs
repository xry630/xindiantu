using System.Windows;
using ECGViewer.Wpf.ViewModels;

namespace ECGViewer.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ExampleViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}