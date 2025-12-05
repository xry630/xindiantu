using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ECGViewer.Wpf.Services;
using ECGViewer.Wpf.Timing;

namespace ECGViewer.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        /// <summary>
        /// Gets the current service provider
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Override the application startup to configure dependency injection
        /// </summary>
        /// <param name="e">Startup event arguments</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Configure the host and services
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Register view model base
                    services.AddTransient(typeof(ViewModels.ViewModelBase));
                    
                    // Register command services
                    services.AddTransient<Commands.RelayCommand>();
                    services.AddTransient<Commands.AsyncRelayCommand>();
                    
                    // Register timer abstractions
                    services.AddSingleton<Timing.IDispatcherTimerFactory, Timing.DispatcherTimerFactory>();
                    services.AddTransient<Timing.IDispatcherTimer, Timing.DispatcherTimerAdapter>();
                    
                    // Register dialog services
                    services.AddSingleton<Services.IDialogService, Services.DialogService>();
                    services.AddSingleton<Services.IFileDialogService, Services.FileDialogService>();
                    
                    // Register main window and view models
                    services.AddTransient<MainWindow>();
                    services.AddTransient<ViewModels.ExampleViewModel>();
                })
                .Build();

            ServiceProvider = _host.Services;

            // Create and show the main window
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        /// <summary>
        /// Override the application exit to clean up the host
        /// </summary>
        /// <param name="e">Exit event arguments</param>
        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }

        /// <summary>
        /// Gets a service of the specified type from the DI container
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>An instance of the specified service</returns>
        public static T GetService<T>() where T : class
        {
            return ServiceProvider?.GetService<T>();
        }

        /// <summary>
        /// Gets a service of the specified type from the DI container
        /// </summary>
        /// <param name="serviceType">The type of service to get</param>
        /// <returns>An instance of the specified service</returns>
        public static object GetService(System.Type serviceType)
        {
            return ServiceProvider?.GetService(serviceType);
        }
    }
}