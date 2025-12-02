---
命令模式（Command Pattern）和 MVVM（Model-View-ViewModel）是两个不同的设计模式，但它们在 WPF（Windows Presentation Foundation）程序中的关系和协作非常紧密，尤其是在构建现代、解耦、可维护的桌面应用时。

### 1. **命令模式（Command Pattern）**
命令模式的核心思想是将请求（操作）封装成一个对象，从而允许你将请求的发起者与请求的接收者解耦。这样，你就能够在不暴露具体实现的情况下，执行某些操作。

在 WPF 中，命令模式通常用于：
- 处理用户输入（如按钮点击、菜单选择等）。
- 将 UI 元素的行为（如按钮点击）与实际的业务逻辑解耦。
- 使得命令与 UI 控件的交互更加灵活，例如，可以复用命令对象，或者在不同的界面之间共享逻辑。

例如，WPF 中的 `ICommand` 接口就是命令模式的一个实现，它允许将操作（例如按钮点击时的动作）封装为一个命令对象，放入 ViewModel 中。

### 2. **MVVM（Model-View-ViewModel）**
MVVM 是一种适用于 WPF 和类似技术的设计模式，旨在将界面（View）与业务逻辑（Model）分离，进而促进应用程序的可测试性和可维护性。

- **Model**：表示应用程序的核心业务逻辑或数据。
- **View**：负责展示 UI，通常只关心展示层，监听并响应用户操作。
- **ViewModel**：充当 View 和 Model 之间的中介，负责提供数据和行为的绑定。ViewModel 的职责是向 View 提供所需的状态，并处理业务逻辑。

在 MVVM 模式中，ViewModel 扮演着协调者的角色，处理界面展示和业务逻辑之间的互动。

### 3. **命令模式与 MVVM 的关系**
在 WPF 中，命令模式通常与 MVVM 模式紧密配合，特别是在处理用户界面（UI）交互时。通过 `ICommand` 接口，命令模式为 ViewModel 提供了一种简洁、松耦合的方式来处理用户输入事件。

#### 关键点：
- **View**：View 会通过绑定（Binding）来与 ViewModel 进行交互。通常，ViewModel 会暴露命令属性（如 `ICommand` 类型），这些命令会绑定到按钮、菜单等控件的 `Command` 属性上。
- **ViewModel**：ViewModel 中的命令处理逻辑封装了具体的操作，如按钮点击事件的处理。通过命令对象，ViewModel 可以清晰地管理视图中的用户操作逻辑，而无需直接操作 UI 元素。
- **解耦**：命令模式使得 View 和 ViewModel 之间的解耦更加彻底，View 不需要知道具体的操作如何实现，只需要知道如何调用命令对象，而 ViewModel 则专注于逻辑和状态管理，不需要关心 UI 控件的具体实现。

#### 举个例子：
假设有一个按钮点击事件，执行一些复杂的业务逻辑。你可以将这部分业务逻辑封装成一个 `ICommand` 的实现，然后在 ViewModel 中暴露这个命令属性，View 通过数据绑定将按钮的点击事件绑定到该命令。当用户点击按钮时，命令对象会调用 ViewModel 中定义的处理方法。

### 4. **编程思路**
- **简化 UI 与业务逻辑的耦合**：使用命令模式，避免直接在 UI 控件中处理事件逻辑，而是将事件处理委托给 ViewModel 的命令。这样，UI 代码变得更加简洁，业务逻辑得以集中处理。
- **增强可测试性**：通过将逻辑封装成命令，可以轻松地对命令进行单元测试，而无需涉及 UI 控件。
- **灵活性和可扩展性**：命令模式使得用户交互可以通过不同的命令对象进行扩展和组合。例如，多个按钮可以共享相同的命令，而每个命令又可以根据不同的条件执行不同的操作。

### 总结：
命令模式和 MVVM 结合使得 WPF 应用程序能够更好地实现解耦和模块化。命令模式提供了一种通过命令对象来管理 UI 交互的方式，而 MVVM 模式则提供了一种清晰的结构来组织数据和行为。两者的结合不仅能使代码更加简洁、可维护，还能提升应用程序的可测试性。

#### 示例：
假设有一个简单的应用，用户点击一个按钮，按钮触发一个命令来执行某个操作。

1. **Model：**
   ```csharp
   public class MyModel
   {
       public string Data { get; set; }
   }
   ```

2. **ViewModel：**
   ```csharp
   public class MyViewModel : INotifyPropertyChanged
   {
       private ICommand _myCommand;
       public ICommand MyCommand
       {
           get
           {
               if (_myCommand == null)
               {
                   _myCommand = new RelayCommand(ExecuteCommand);
               }
               return _myCommand;
           }
       }

       private void ExecuteCommand(object parameter)
       {
           // 业务逻辑处理
           MessageBox.Show("Command Executed!");
       }

       public event PropertyChangedEventHandler PropertyChanged;
   }
   ```

3. **RelayCommand 类：**
   这是一个常见的实现 `ICommand` 接口的类，处理命令的执行和是否可以执行的逻辑。

   ```csharp
   public class RelayCommand : ICommand
   {
       private readonly Action<object> _execute;
       private readonly Predicate<object> _canExecute;

       public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
       {
           _execute = execute ?? throw new ArgumentNullException(nameof(execute));
           _canExecute = canExecute;
       }

       public bool CanExecute(object parameter)
       {
           return _canExecute?.Invoke(parameter) ?? true;
       }

       public void Execute(object parameter)
       {
           _execute(parameter);
       }

       public event EventHandler CanExecuteChanged
       {
           add => CommandManager.RequerySuggested += value;
           remove => CommandManager.RequerySuggested -= value;
       }
   }
   ```

4. **View（XAML）:**
   ```xml
   <Button Content="Click Me" Command="{Binding MyCommand}" />
   ```

### 总结：
- **命令模式** 在 WPF 中通过 `ICommand` 接口的实现，解耦了 UI 控件与具体的事件处理逻辑。
- **MVVM** 通过 ViewModel 将业务逻辑与视图分离，使用命令来处理 UI 与 ViewModel 之间的交互。

结合使用命令模式和 MVVM，可以让 WPF 应用程序具有更清晰、更易维护的结构，特别是当项目变得复杂时，这种解耦会极大提高代码的可测试性和可扩展性。
---

```
├── WpfApp1/                         # 项目根目录
│   ├── WpfApp1/                            # 主要 WPF 项目文件夹
│   │   ├── App.config                      # 应用程序配置文件
│   │   ├── App.xaml                        # WPF 应用程序入口 XAML
│   │   ├── App.xaml.cs                     # WPF 应用程序入口代码
│   │   ├── MainWindow.xaml                  # 主窗口 XAML
│   │   ├── MainWindow.xaml.cs               # 主窗口后台代码

│   │   ├── Base/                            # 基础功能类
│   │   │   ├── DelegateCommand.cs           # 命令模式类
│   │   │   ├── NotificationObject.cs        # 通知对象基类

│   │   ├── Connected Services/              # 连接的服务（Web API 等）
│   │   ├── GlobalPara.cs                    # 全局参数管理

│   │   ├── Models/                          # 数据模型
│   │   │   ├── AsciiJson.cs                 # ASCII JSON 处理类
│   │   │   ├── ChartParas.cs                # 图表参数
│   │   │   ├── DisplayPara.cs               # 显示参数
│   │   │   ├── FastCmdModel.cs              # 快速命令模型
│   │   │   ├── FastCmdsCfg.cs               # 快速命令配置
│   │   │   ├── LogPara.cs                   # 日志参数
│   │   │   ├── MeasureData.cs               # 测量数据
│   │   │   ├── mycfg.cs                     # 配置文件
│   │   │   ├── ReceivePara.cs               # 接收参数
│   │   │   ├── SendPara.cs                  # 发送参数

│   │   ├── Util/                            # 工具类
│   │   │   ├── DataConvertUtility.cs        # 数据转换工具
│   │   │   ├── JSONHelper.cs                # JSON 处理工具
│   │   │   ├── StringCheck.cs               # 字符串检查工具
│   │   │   ├── Tool.cs                      # 通用工具类

│   │   ├── ViewModels/                      # 视图模型
│   │   │   ├── ChartViewModel.cs            # 图表视图模型




首次创建时生成,
基于时间窗口动态移除旧数据，固定 X 轴范围,保留 最近60秒
```










