using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ECGViewer.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register ViewModels
            // services.AddTransient<MainWindowViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Wire up global exception handling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Resolve and show MainWindow
            var mainWindow = _host?.Services.GetRequiredService<MainWindow>();
            if (mainWindow != null)
            {
                mainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ShowErrorDialog("An unexpected error occurred", ex?.Message ?? "Unknown error");
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ShowErrorDialog("An error occurred in the application", e.Exception.Message);
            e.Handled = true;
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ShowErrorDialog("An error occurred in a background task", e.Exception.InnerException?.Message ?? e.Exception.Message);
            e.SetObserved();
        }

        private void ShowErrorDialog(string title, string? message)
        {
            MessageBox.Show(
                message ?? "An unknown error occurred. Please check the application logs.",
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
