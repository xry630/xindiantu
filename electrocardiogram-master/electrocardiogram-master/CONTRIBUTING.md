# Contributing to ECG Viewer

## Development Setup

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Building the Solution

The recommended approach is to use the `ECGViewer.sln` solution file:

```bash
cd electrocardiogram-master
dotnet build ECGViewer.sln
```

### Running the Application

To run the modern WPF application:

```bash
dotnet run --project ECGViewer.Wpf/ECGViewer.Wpf.csproj
```

Or if you have Visual Studio installed:
1. Open `ECGViewer.sln`
2. Set `ECGViewer.Wpf` as the startup project
3. Press `F5` to run

## Project Architecture

### Modern WPF Entry Point (ECGViewer.Wpf)

The `ECGViewer.Wpf` project is the recommended entry point for new development. It features:

#### 1. Dependency Injection Setup
Located in `App.xaml.cs`, the DI container is configured during application startup:

```csharp
Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        ConfigureServices(services);
    })
    .Build();
```

Add your services to the `ConfigureServices` method:
```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Example:
    // services.AddTransient<IDataService, DataService>();
    // services.AddTransient<MainWindowViewModel>();
    services.AddTransient<MainWindow>();
}
```

#### 2. Global Exception Handling
The application has three layers of exception handling:

- **UI Thread Exceptions**: `DispatcherUnhandledException`
- **Unhandled Exceptions**: `AppDomain.CurrentDomain.UnhandledExceptionEvent`
- **Background Task Exceptions**: `TaskScheduler.UnobservedTaskException`

All exceptions display a user-friendly message box.

#### 3. MVVM Support
The project includes `CommunityToolkit.Mvvm` for lightweight MVVM support. Use `ObservableObject` and `RelayCommand` for viewmodels:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string myProperty = "value";

    [RelayCommand]
    private void MyCommand()
    {
        // Handle command
    }
}
```

### Extending with Core and Infrastructure Layers

The WPF project is designed to reference optional Core and Infrastructure projects:

```xml
<ItemGroup>
    <ProjectReference Include="../../../ECGViewer.Core/ECGViewer.Core.csproj" 
                      Condition="Exists('../../../ECGViewer.Core/ECGViewer.Core.csproj')" />
    <ProjectReference Include="../../../ECGViewer.Infrastructure/ECGViewer.Infrastructure.csproj" 
                      Condition="Exists('../../../ECGViewer.Infrastructure/ECGViewer.Infrastructure.csproj')" />
</ItemGroup>
```

These projects should provide:
- **ECGViewer.Core**: Business logic, models, and algorithms
- **ECGViewer.Infrastructure**: Data access, file I/O, and external service integration

The guarded references (`Condition="Exists(...)"`) ensure the build succeeds even if these projects don't exist yet.

### Legacy WinForms Application (MsPaint)

The `MsPaint` project is the original WinForms implementation. It is maintained for backward compatibility only and should not receive new features.

## Code Style Guidelines

- Follow Microsoft C# naming conventions
- Use nullable reference types (`#nullable enable`)
- Enable analyzer warnings and fix them before committing
- Use `async`/`await` for I/O and long-running operations
- Prefer dependency injection over static references

## Creating New Features

When adding a new feature:

1. If it's UI-related, create a new View and ViewModel in `ECGViewer.Wpf`
2. If it's business logic, consider placing it in `ECGViewer.Core` (if it exists)
3. If it requires data access or I/O, place it in `ECGViewer.Infrastructure` (if it exists)
4. Register services in `App.xaml.cs` via dependency injection
5. Update the UI by binding to the ViewModel

## Testing

Currently, there are no automated tests in the solution. Consider adding a test project:

```bash
dotnet new xunit -n ECGViewer.Tests
```

Then add it to the solution:

```bash
dotnet sln ECGViewer.sln add ECGViewer.Tests/ECGViewer.Tests.csproj
```

## Building for Release

```bash
dotnet publish ECGViewer.Wpf/ECGViewer.Wpf.csproj -c Release -o ./publish
```

This creates a self-contained executable in the `publish` directory.

## Troubleshooting

### Build fails with "Project not found"
This is expected if `ECGViewer.Core` or `ECGViewer.Infrastructure` don't exist yet. The guarded project references prevent the build from failing in this case.

### Runtime error "Unable to resolve service"
A service is being requested from the DI container but wasn't registered. Add it to `ConfigureServices` in `App.xaml.cs`.

### XAML Designer doesn't work in Visual Studio
Ensure you're using Visual Studio 2022 with the WPF designer component installed.

## Questions?

Refer to the project README.md for architecture overview and usage instructions.
