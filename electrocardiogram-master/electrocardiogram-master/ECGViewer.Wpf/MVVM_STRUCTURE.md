# MVVM 基础设施结构

本文档说明 ECGViewer.Wpf 项目中 MVVM 基础设施的文件夹组织和使用方式。

## 文件夹结构

```
ECGViewer.Wpf/
├── App.xaml                     # 应用程序入口 XAML
├── App.xaml.cs                  # 应用程序启动和 DI 配置
├── MainWindow.xaml              # 主窗口 XAML
├── MainWindow.xaml.cs           # 主窗口后端代码
├── ECGViewer.Wpf.csproj         # 项目文件
│
├── ViewModels/                  # 所有视图模型
│   ├── ViewModelBase.cs         # 视图模型基类（继承自 ObservableObject）
│   └── MainWindowViewModel.cs   # 主窗口视图模型
│
├── Views/                       # 所有 XAML 视图
│   └── .gitkeep                 # 占位符文件（用于 git 追踪空文件夹）
│
├── Models/                      # 业务数据模型
│   └── ECGData.cs               # ECG 数据模型
│
├── Services/                    # 应用程序服务
│   ├── IDataService.cs          # 数据服务接口
│   └── DataService.cs           # 数据服务实现
│
├── Utilities/                   # 实用工具类
│   └── Constants.cs             # 应用程序常量
│
└── MVVM_STRUCTURE.md            # 本文件
```

## 各文件夹说明

### ViewModels 文件夹

存放所有的视图模型。每个视图通常对应一个视图模型。

**最佳实践:**
- 继承 `ViewModelBase` 或直接继承 `ObservableObject`
- 使用 `[ObservableProperty]` 特性进行数据绑定
- 使用 `[RelayCommand]` 特性定义命令
- 避免在视图模型中放置 UI 逻辑

**示例:**
```csharp
public partial class MyViewModel : ViewModelBase
{
    [ObservableProperty]
    private string myProperty = "initial value";

    [RelayCommand]
    private void DoSomething()
    {
        // 处理命令逻辑
    }
}
```

### Views 文件夹

存放所有的 XAML 视图文件和后端代码。每个视图通常包含两个文件：
- `SomethingView.xaml` - XAML 标记
- `SomethingView.xaml.cs` - 后端代码

**最佳实践:**
- 将 DataContext 设置为对应的 ViewModel
- 避免在后端代码中放置业务逻辑
- 使用数据绑定连接 UI 和 ViewModel

### Models 文件夹

存放业务数据模型。这些类表示应用程序处理的核心数据。

**最佳实践:**
- 保持模型简洁，只包含数据属性
- 不要在模型中放置业务逻辑
- 使用不可变属性或实现 `INotifyPropertyChanged`（如果需要绑定）

### Services 文件夹

存放应用程序服务，如数据访问、文件操作等。

**最佳实践:**
- 为每个服务创建一个接口（`IServiceName.cs`）
- 在接口中定义公共契约
- 在实现类中提供具体实现
- 通过依赖注入在 App.xaml.cs 中注册服务

**服务注册示例:**
```csharp
private void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IDataService, DataService>();
    services.AddTransient<MyViewModel>();
}
```

**服务注入到 ViewModel:**
```csharp
public partial class MyViewModel : ViewModelBase
{
    private readonly IDataService _dataService;

    public MyViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }
}
```

### Utilities 文件夹

存放应用程序级别的实用工具类，如常量、扩展方法、转换器等。

**包含的内容:**
- `Constants.cs` - 应用程序常量
- `ExtensionMethods.cs` - 扩展方法（可选）
- `Converters.cs` - XAML 值转换器（可选）
- `Validators.cs` - 验证工具（可选）

## 依赖注入工作流

1. **在 App.xaml.cs 中注册服务:**
   ```csharp
   private void ConfigureServices(IServiceCollection services)
   {
       services.AddTransient<IDataService, DataService>();
       services.AddTransient<MyViewModel>();
       services.AddTransient<MainWindow>();
   }
   ```

2. **在 ViewModel 中请求依赖:**
   ```csharp
   public class MyViewModel : ViewModelBase
   {
       private readonly IDataService _dataService;

       public MyViewModel(IDataService dataService)
       {
           _dataService = dataService;
       }
   }
   ```

3. **在 View 中请求 ViewModel:**
   ```csharp
   public partial class MyView : UserControl
   {
       public MyView(MyViewModel viewModel)
       {
           InitializeComponent();
           DataContext = viewModel;
       }
   }
   ```

## 创建新功能的步骤

1. **创建数据模型** - 在 `Models/` 中创建模型类
2. **创建服务接口** - 在 `Services/` 中创建接口
3. **实现服务** - 在 `Services/` 中创建实现类
4. **创建 ViewModel** - 在 `ViewModels/` 中创建视图模型
5. **创建 View** - 在 `Views/` 中创建 XAML 视图和后端代码
6. **注册依赖** - 在 `App.xaml.cs` 的 `ConfigureServices` 中注册
7. **绑定数据** - 在 XAML 中绑定 ViewModel 属性

## CommunityToolkit.Mvvm 快速参考

### 属性通知
```csharp
[ObservableProperty]
private string message = "Hello";
```
生成的属性: `public string Message { get; set; }`

### 命令
```csharp
[RelayCommand]
private void MyCommand()
{
    // 执行
}

[RelayCommand(CanExecute = nameof(CanExecute))]
private void ConditionalCommand()
{
    // 有条件的执行
}

private bool CanExecute()
{
    return true;
}
```

### 异步命令
```csharp
[RelayCommand]
private async Task LoadDataAsync()
{
    // 异步操作
    await Task.Delay(1000);
}
```

## 最佳实践总结

1. ✅ 保持关注点分离（SoC）
2. ✅ 使用依赖注入而不是静态引用
3. ✅ 在 ViewModel 中使用异步操作
4. ✅ 使用数据绑定而不是代码交互
5. ✅ 避免在 View 的后端代码中放置业务逻辑
6. ✅ 为服务创建接口以便测试
7. ✅ 使用常量避免硬编码值

## 参考资源

- [CommunityToolkit.Mvvm 文档](https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/)
- [Microsoft Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [MVVM 设计模式](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)
