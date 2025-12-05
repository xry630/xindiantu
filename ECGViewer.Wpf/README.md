# ECGViewer.Wpf MVVM Infrastructure

This project provides a comprehensive MVVM infrastructure for WPF applications, designed to support dependency injection, validation, and testability.

## Architecture Overview

The MVVM infrastructure is organized into the following key components:

### Core Components

#### 1. ViewModels/ViewModelBase.cs
- Implements `INotifyPropertyChanged` with `SetProperty` helper methods
- Implements `INotifyDataErrorInfo` for validation support
- Provides dispatcher marshalling for UI thread operations
- Includes validation dictionary and error management
- Implements `IDisposable` for proper resource cleanup

**Key Features:**
- `SetProperty<T>()` - Property change notification with value comparison
- `SetPropertyAndValidate<T>()` - Property change with automatic validation
- `ValidateProperty()` - Overrideable validation method
- `AddError()`, `ClearErrors()` - Error management
- `InvokeOnMainThread()` - Thread-safe UI operations

#### 2. Commands/RelayCommand.cs
- `RelayCommand` - Synchronous command implementation
- `AsyncRelayCommand` - Asynchronous command with execution state management
- `RelayCommand<T>` - Generic version for strongly-typed parameters
- `AsyncRelayCommand<T>` - Generic async version
- Exception handling support
- `CanExecute` predicate support
- Integration with `SynchronizationContext` for UI responsiveness

**Key Features:**
- Automatic `CanExecute` re-evaluation during async execution
- Exception handling with optional error callbacks
- Thread-safe command execution
- Support for both parameter and parameterless commands

#### 3. Timing/ (Timer Abstractions)
- `IDispatcherTimer` - Timer interface for dependency injection
- `IDispatcherTimerFactory` - Factory for creating timers
- `DispatcherTimerAdapter` - WPF DispatcherTimer wrapper
- `DispatcherTimerFactory` - WPF implementation of factory

**Benefits:**
- Enables unit testing of timer-dependent view models
- Decouples view models from concrete WPF types
- Supports dependency injection patterns

#### 4. Services/ (Dialog Services)
- `IDialogService` - Interface for message dialogs
- `IFileDialogService` - Interface for file operations
- `DialogService` - WPF MessageBox implementation
- `FileDialogService` - WPF file dialog implementation

**Features:**
- Async dialog operations
- Support for various dialog types (Information, Warning, Error, Confirmation)
- File open/save dialogs with filtering
- Folder selection dialogs
- Decouples view models from UI-specific dialog APIs

## Dependency Injection Setup

The infrastructure uses Microsoft.Extensions.DependencyInjection for service registration. All components are registered in `App.xaml.cs`:

```csharp
services.AddTransient<ViewModels.ViewModelBase>();
services.AddTransient<Commands.RelayCommand>();
services.AddTransient<Commands.AsyncRelayCommand>();
services.AddSingleton<IDispatcherTimerFactory, DispatcherTimerFactory>();
services.AddTransient<IDispatcherTimer, DispatcherTimerAdapter>();
services.AddSingleton<IDialogService, DialogService>();
services.AddSingleton<IFileDialogService, FileDialogService>();
```

## Usage Examples

### Creating a View Model

```csharp
public class MyViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IDispatcherTimer _timer;
    private string _status;

    public MyViewModel(IDialogService dialogService, IDispatcherTimerFactory timerFactory)
    {
        _dialogService = dialogService;
        _timer = timerFactory.CreateTimer();
        
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        ShowMessageCommand = new RelayCommand(ShowMessage);
        
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public AsyncRelayCommand LoadDataCommand { get; }
    public RelayCommand ShowMessageCommand { get; }

    private async Task LoadDataAsync()
    {
        try
        {
            Status = "Loading...";
            // Load data asynchronously
            Status = "Complete";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(ex.Message);
        }
    }

    private void ShowMessage()
    {
        _dialogService.ShowInformationAsync(Status);
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        // Timer logic here
    }
}
```

### Validation Example

```csharp
protected override void ValidateProperty(string propertyName, object value)
{
    ClearErrors(propertyName);

    switch (propertyName)
    {
        case nameof(Email):
            if (value is string email && !IsValidEmail(email))
            {
                AddError(propertyName, "Invalid email format");
            }
            break;
        case nameof(Age):
            if (value is int age && (age < 0 || age > 120))
            {
                AddError(propertyName, "Age must be between 0 and 120");
            }
            break;
    }
}
```

## Testing Support

The infrastructure is designed for testability:

### Mocking Services
```csharp
// In unit tests
var mockDialogService = new Mock<IDialogService>();
var mockTimerFactory = new Mock<IDispatcherTimerFactory>();
var mockTimer = new Mock<IDispatcherTimer>();

mockTimerFactory.Setup(x => x.CreateTimer()).Returns(mockTimer.Object);

var viewModel = new MyViewModel(mockDialogService.Object, mockTimerFactory.Object);
```

### Timer Testing
```csharp
// Test timer-dependent behavior
mockTimer.Raise(x => x.Tick += null, EventArgs.Empty);
Assert.AreEqual("Timer ticked", viewModel.Status);
```

## Benefits

1. **Separation of Concerns** - View models are decoupled from UI-specific implementations
2. **Testability** - All dependencies can be mocked for unit testing
3. **Reusability** - Common MVVM patterns are abstracted into reusable components
4. **Maintainability** - Clear separation between view logic and business logic
5. **Extensibility** - Easy to add new services and extend existing functionality
6. **Thread Safety** - Built-in support for UI thread marshalling
7. **Validation** - Integrated validation framework with INotifyDataErrorInfo

## Future Enhancements

The infrastructure is ready for potential adoption of MVVM Community Toolkit while maintaining the custom scaffolding as a fallback. The design allows for:

- Easy migration to toolkit components
- Custom implementations where toolkit doesn't meet requirements
- Gradual adoption of toolkit features
- Backward compatibility with existing code

## Project Structure

```
ECGViewer.Wpf/
├── ViewModels/
│   ├── ViewModelBase.cs          # Base view model with INotifyPropertyChanged and validation
│   └── ExampleViewModel.cs      # Example implementation
├── Commands/
│   └── RelayCommand.cs          # Command implementations with async support
├── Timing/
│   ├── IDispatcherTimer.cs       # Timer interface
│   ├── IDispatcherTimerFactory.cs # Timer factory interface
│   ├── DispatcherTimerAdapter.cs # WPF timer wrapper
│   └── DispatcherTimerFactory.cs # WPF timer factory
├── Services/
│   ├── IDialogService.cs        # Dialog service interface
│   ├── IFileDialogService.cs    # File dialog service interface
│   ├── DialogService.cs         # WPF dialog implementation
│   └── FileDialogService.cs     # WPF file dialog implementation
├── App.xaml                      # Application definition
├── App.xaml.cs                   # DI container setup
├── MainWindow.xaml               # Main window XAML
├── MainWindow.xaml.cs            # Main window code-behind
└── ECGViewer.Wpf.csproj         # Project file
```

This infrastructure provides a solid foundation for building maintainable, testable WPF applications using the MVVM pattern.