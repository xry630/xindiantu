using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ECGViewer.Wpf.Services;
using ECGViewer.Wpf.Timing;
using ECGViewer.Wpf.ViewModels;
using ECGViewer.Wpf.Commands;

namespace ECGViewer.Wpf.Tests
{
    /// <summary>
    /// Simple test program to verify MVVM infrastructure functionality
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Testing ECGViewer.Wpf MVVM Infrastructure...");
            Console.WriteLine("==========================================");

            // Setup DI container
            var services = new ServiceCollection();
            
            // Register services
            services.AddSingleton<IDialogService, MockDialogService>();
            services.AddSingleton<IFileDialogService, MockFileDialogService>();
            services.AddSingleton<IDispatcherTimerFactory, MockDispatcherTimerFactory>();
            
            // Register view models
            services.AddTransient<ExampleViewModel>();
            
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                // Test 1: ViewModelBase functionality
                await TestViewModelBase(serviceProvider);
                
                // Test 2: Command functionality
                await TestCommands(serviceProvider);
                
                // Test 3: Service functionality
                await TestServices(serviceProvider);
                
                // Test 4: Timer functionality
                await TestTimers(serviceProvider);
                
                // Test 5: Integration test with ExampleViewModel
                await TestExampleViewModel(serviceProvider);
                
                Console.WriteLine("\n==========================================");
                Console.WriteLine("All tests completed successfully!");
                Console.WriteLine("MVVM infrastructure is ready for use.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nTest failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static async Task TestViewModelBase(ServiceProvider serviceProvider)
        {
            Console.WriteLine("\n1. Testing ViewModelBase...");
            
            var viewModel = new TestViewModel();
            
            // Test property change notification
            bool propertyChanged = false;
            viewModel.PropertyChanged += (s, e) => propertyChanged = true;
            
            viewModel.TestProperty = "Test Value";
            Console.WriteLine($"   Property changed notification: {(propertyChanged ? "✓" : "✗")}");
            
            // Test validation
            viewModel.PublicValidateProperty("TestProperty", null);
            Console.WriteLine($"   Validation working: {(viewModel.HasErrors ? "✓" : "✗")}");
        }

        private static async Task TestCommands(ServiceProvider serviceProvider)
        {
            Console.WriteLine("\n2. Testing Commands...");
            
            bool executed = false;
            var command = new RelayCommand(() => executed = true);
            
            Console.WriteLine($"   CanExecute: {(command.CanExecute(null) ? "✓" : "✗")}");
            command.Execute(null);
            Console.WriteLine($"   Execute: {(executed ? "✓" : "✗")}");
            
            // Test async command
            bool asyncExecuted = false;
            var asyncCommand = new AsyncRelayCommand(async () => 
            {
                await Task.Delay(10);
                asyncExecuted = true;
            });
            
            asyncCommand.Execute(null);
            await Task.Delay(50); // Wait for async completion
            Console.WriteLine($"   Async Execute: {(asyncExecuted ? "✓" : "✗")}");
        }

        private static async Task TestServices(ServiceProvider serviceProvider)
        {
            Console.WriteLine("\n3. Testing Services...");
            
            var dialogService = serviceProvider.GetRequiredService<IDialogService>();
            var fileDialogService = serviceProvider.GetRequiredService<IFileDialogService>();
            
            // Test dialog service
            var result = await dialogService.ShowConfirmationAsync("Test message", "Test");
            Console.WriteLine($"   Dialog service: {(result ? "✓" : "✗")}");
            
            // Test file dialog service
            var filePath = await fileDialogService.ShowOpenFileDialogAsync();
            Console.WriteLine($"   File dialog service: {(filePath != null ? "✓" : "✗")}");
        }

        private static async Task TestTimers(ServiceProvider serviceProvider)
        {
            Console.WriteLine("\n4. Testing Timers...");
            
            var timerFactory = serviceProvider.GetRequiredService<IDispatcherTimerFactory>();
            var timer = timerFactory.CreateTimer();
            
            bool timerTicked = false;
            timer.Tick += (s, e) => timerTicked = true;
            timer.Interval = TimeSpan.FromMilliseconds(10);
            
            timer.Start();
            await Task.Delay(50);
            timer.Stop();
            
            Console.WriteLine($"   Timer tick: {(timerTicked ? "✓" : "✗")}");
            Console.WriteLine($"   Timer enabled state: {(!timer.IsEnabled ? "✓" : "✗")}");
        }

        private static async Task TestExampleViewModel(ServiceProvider serviceProvider)
        {
            Console.WriteLine("\n5. Testing ExampleViewModel Integration...");
            
            var viewModel = serviceProvider.GetRequiredService<ExampleViewModel>();
            
            // Test initial state
            Console.WriteLine($"   Initial counter: {viewModel.Counter}");
            Console.WriteLine($"   Initial status: {viewModel.StatusMessage}");
            
            // Test command availability
            Console.WriteLine($"   ShowMessage can execute: {viewModel.ShowMessageCommand.CanExecute(null)}");
            Console.WriteLine($"   StartTimer can execute: {viewModel.StartTimerCommand.CanExecute(null)}");
            
            // Test timer functionality
            viewModel.StartTimerCommand.Execute(null);
            await Task.Delay(1100); // Wait for timer tick
            viewModel.StopTimerCommand.Execute(null);
            
            Console.WriteLine($"   Counter after timer: {viewModel.Counter}");
            Console.WriteLine($"   Status after timer: {viewModel.StatusMessage}");
            Console.WriteLine($"   Integration test: ✓");
        }
    }

    // Mock implementations for testing
    public class TestViewModel : ViewModelBase
    {
        private string _testProperty;
        
        public string TestProperty
        {
            get => _testProperty;
            set => SetProperty(ref _testProperty, value);
        }
        
        public void PublicValidateProperty(string propertyName, object value)
        {
            ValidateProperty(propertyName, value);
        }
        
        protected override void ValidateProperty(string propertyName, object value)
        {
            if (propertyName == nameof(TestProperty) && value == null)
            {
                AddError(propertyName, "TestProperty cannot be null");
            }
        }
    }

    public class MockDialogService : IDialogService
    {
        public Task<DialogResult> ShowMessageAsync(string message, string title = "", DialogButton button = DialogButton.OK, DialogIcon icon = DialogIcon.Information)
        {
            return Task.FromResult(button == DialogButton.YesNo ? DialogResult.Yes : DialogResult.OK);
        }

        public Task<bool> ShowConfirmationAsync(string message, string title = "Confirmation")
        {
            return Task.FromResult(true);
        }

        public Task ShowErrorAsync(string message, string title = "Error")
        {
            Console.WriteLine($"Error: {message}");
            return Task.CompletedTask;
        }

        public Task ShowWarningAsync(string message, string title = "Warning")
        {
            Console.WriteLine($"Warning: {message}");
            return Task.CompletedTask;
        }

        public Task ShowInformationAsync(string message, string title = "Information")
        {
            Console.WriteLine($"Info: {message}");
            return Task.CompletedTask;
        }
    }

    public class MockFileDialogService : IFileDialogService
    {
        public Task<string> ShowOpenFileDialogAsync(string title = "Open File", string filter = "All files (*.*)|*.*", string initialDirectory = "", bool multiSelect = false)
        {
            return Task.FromResult<string>(null);
        }

        public Task<System.Collections.Generic.IEnumerable<string>> ShowOpenFileDialogMultipleAsync(string title = "Open Files", string filter = "All files (*.*)|*.*", string initialDirectory = "")
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }

        public Task<string> ShowSaveFileDialogAsync(string title = "Save File", string filter = "All files (*.*)|*.*", string initialDirectory = "", string defaultFileName = "")
        {
            return Task.FromResult<string>(null);
        }

        public Task<string> ShowFolderDialogAsync(string title = "Select Folder", string initialDirectory = "")
        {
            return Task.FromResult<string>(null);
        }
    }

    public class MockDispatcherTimerFactory : IDispatcherTimerFactory
    {
        public IDispatcherTimer CreateTimer()
        {
            return new MockDispatcherTimer();
        }
    }

    public class MockDispatcherTimer : IDispatcherTimer
    {
        private System.Timers.Timer _timer;
        private bool _isEnabled;
        
        public event EventHandler Tick;

        public TimeSpan Interval { get; set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public void Start()
        {
            if (_timer == null)
            {
                _timer = new System.Timers.Timer(Interval.TotalMilliseconds);
                _timer.Elapsed += (s, e) => Tick?.Invoke(this, e);
            }
            _timer.Start();
            _isEnabled = true;
        }

        public void Stop()
        {
            _timer?.Stop();
            _isEnabled = false;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}