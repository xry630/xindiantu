# ECG Viewer Applications Collection

This repository contains a collection of physiological signal visualization applications built with .NET technologies.

## Projects Overview

### 1. ECGViewer.Wpf (WPF MVVM Infrastructure)
A complete MVVM infrastructure project for WPF applications with comprehensive dependency injection setup.

**Key Features:**
- ViewModelBase implementing INotifyPropertyChanged and INotifyDataErrorInfo
- RelayCommand with async variants and strong typing support
- Timer abstractions (IDispatcherTimer, IDispatcherTimerFactory) for testability
- Dialog services (IDialogService, IFileDialogService) for UI decoupling
- Full DI container setup using Microsoft.Extensions.DependencyInjection
- Validation support with error management
- Thread-safe UI operations

**Location:** `/ECGViewer.Wpf`

**To Run:**
```bash
cd ECGViewer.Wpf
dotnet run
```

**Key Components:**
- `ViewModels/ViewModelBase.cs` - Base view model with property notification and validation
- `Commands/RelayCommand.cs` - Command implementations with async support
- `Timing/` - Timer abstractions for dependency injection
- `Services/` - Dialog and file dialog services
- `App.xaml.cs` - DI container configuration

### 2. WpfApp1 (WPF LiveCharts Application)
A WPF application using LiveCharts for real-time measurement curve visualization.

**Location:** `/WpfApp1`

### 3. draw-curve-demo (WPF Custom Chart Demo)
A WPF sample showcasing a custom fetal heart rate chart with manual drawing capabilities.

**Location:** `/draw-curve-demo-master`

### 4. electrocardiogram-master (WinForms ECG Viewer)
A WinForms application for ECG signal visualization with Q/R/T feature detection.

**Location:** `/electrocardiogram-master`

## Project Structure

```
project-root/
├── ECGViewer.Wpf/                    # MVVM Infrastructure
│   ├── ViewModels/
│   │   ├── ViewModelBase.cs
│   │   └── ExampleViewModel.cs
│   ├── Commands/
│   │   └── RelayCommand.cs
│   ├── Timing/
│   │   ├── IDispatcherTimer.cs
│   │   ├── DispatcherTimerAdapter.cs
│   │   ├── IDispatcherTimerFactory.cs
│   │   └── DispatcherTimerFactory.cs
│   ├── Services/
│   │   ├── IDialogService.cs
│   │   ├── DialogService.cs
│   │   ├── IFileDialogService.cs
│   │   └── FileDialogService.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── ECGViewer.Wpf.csproj
│   ├── ECGViewer.Wpf.sln
│   ├── README.md
│   └── IMPLEMENTATION_SUMMARY.md
│
├── WpfApp1/                          # WPF LiveCharts
│   └── ...
│
├── draw-curve-demo-master/           # WPF Custom Chart
│   └── ...
│
├── electrocardiogram-master/         # WinForms ECG
│   └── ...
│
├── .gitignore
└── README.md                          # This file
```

## Getting Started

### Prerequisites
- .NET 6.0 or higher
- Visual Studio 2022 or Visual Studio Code
- Git

### Building

```bash
# Build ECGViewer.Wpf
cd ECGViewer.Wpf
dotnet build
```

### Running

```bash
# Run ECGViewer.Wpf
cd ECGViewer.Wpf
dotnet run
```

### Running Tests

```bash
cd ECGViewer.Wpf
dotnet run Tests/Program.cs
```

## MVVM Infrastructure Features

### ViewModelBase
- Property change notification with `SetProperty` helper methods
- Property change notification with automatic validation using `SetPropertyAndValidate`
- Validation support with `INotifyDataErrorInfo`
- Error management with `AddError` and `ClearErrors` methods
- Thread-safe UI operations with `InvokeOnMainThread`
- Resource disposal support with `IDisposable`

### Commands
- **RelayCommand**: Synchronous command implementation
- **AsyncRelayCommand**: Asynchronous command with execution state management
- **RelayCommand<T>**: Generic version for strongly-typed parameters
- **AsyncRelayCommand<T>**: Generic async version
- Support for `CanExecute` predicates
- Exception handling support
- SynchronizationContext integration for UI responsiveness

### Timer Abstractions
- **IDispatcherTimer**: Timer interface for dependency injection
- **IDispatcherTimerFactory**: Factory for creating timers
- Enables unit testing of timer-dependent view models
- Decouples view models from concrete WPF types

### Dialog Services
- **IDialogService**: Interface for message dialogs
  - ShowMessageAsync
  - ShowConfirmationAsync
  - ShowErrorAsync
  - ShowWarningAsync
  - ShowInformationAsync

- **IFileDialogService**: Interface for file operations
  - ShowOpenFileDialogAsync
  - ShowOpenFileDialogMultipleAsync
  - ShowSaveFileDialogAsync
  - ShowFolderDialogAsync

## Usage Example

```csharp
public class MyViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IDispatcherTimer _timer;
    private string _status;

    public MyViewModel(
        IDialogService dialogService,
        IDispatcherTimerFactory timerFactory)
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
            await Task.Delay(1000); // Simulate work
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
        Status = $"Timer tick: {DateTime.Now:HH:mm:ss}";
    }
}
```

## Validation Example

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

All components are designed for testability with mock-friendly interfaces:

```csharp
// In unit tests
var mockDialogService = new Mock<IDialogService>();
var mockTimerFactory = new Mock<IDispatcherTimerFactory>();
var mockTimer = new Mock<IDispatcherTimer>();

mockTimerFactory.Setup(x => x.CreateTimer()).Returns(mockTimer.Object);

var viewModel = new MyViewModel(mockDialogService.Object, mockTimerFactory.Object);

// Test timer-dependent behavior
mockTimer.Raise(x => x.Tick += null, EventArgs.Empty);
Assert.AreEqual("expected_value", viewModel.Property);
```

## Benefits

1. **Separation of Concerns** - View models are decoupled from UI-specific implementations
2. **Testability** - All dependencies can be mocked for unit testing
3. **Reusability** - Common MVVM patterns are abstracted into reusable components
4. **Maintainability** - Clear separation between view logic and business logic
5. **Extensibility** - Easy to add new services and extend existing functionality
6. **Thread Safety** - Built-in support for UI thread marshalling
7. **Validation** - Integrated validation framework with INotifyDataErrorInfo

## MVVM Community Toolkit Ready

The infrastructure is designed to be **MVVM Community Toolkit ready** while maintaining custom scaffolding:
- Custom implementations can coexist with toolkit components
- Gradual migration path available
- No breaking changes to existing code when toolkit is introduced
- All interfaces follow standard patterns for easy replacement

## Contributing

When contributing to this project, please:
1. Follow the existing code style and conventions
2. Use the MVVM infrastructure provided for view models
3. Use dependency injection for service access
4. Write unit tests for new functionality
5. Update documentation as needed

## License

All projects in this repository are provided as-is for educational and commercial use.

## Project Status

- **ECGViewer.Wpf**: ✅ Complete - MVVM infrastructure with full DI setup
- **WpfApp1**: ✅ Functional - WPF LiveCharts application
- **draw-curve-demo**: ✅ Functional - Custom WPF chart demo
- **electrocardiogram-master**: ✅ Functional - WinForms ECG application

## Support

For issues, questions, or suggestions, please refer to the specific project's documentation or contact the development team.
