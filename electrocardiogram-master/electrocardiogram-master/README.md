# 心电图显示程序 (ECG Viewer)

## 项目简介
这个仓库包含多个心电图显示程序实现：
- **ECGViewer.Wpf** (推荐) - 现代化的WPF(.NET 8)实现，采用依赖注入和MVVM架构
- **MsPaint** (遗留) - 基于C# WinForms的心电图实时显示程序

## 功能特性
- 支持加载TXT格式的心电图数据文件
- 实时显示心电图波形
- 动态网格背景
- 标记R、Q、T波位置
- 计算并显示QT间期和QR间期
- 实时监控QT间期是否在正常范围内

## 数据格式说明
程序支持TXT格式的数据文件，数据格式要求：
- 每行一个数值，表示心电图采样点的电压值
- 采样频率：1000 Hz
- 数据单位：原始单位（需要转换为物理单位）
- 建议数据长度：38400个采样点（约38秒的数据）

## 快速开始

### 使用 ECGViewer.Wpf (推荐)
```bash
cd electrocardiogram-master
dotnet build ECGViewer.sln
dotnet run --project ECGViewer.Wpf/ECGViewer.Wpf.csproj
```

### 使用 MsPaint (遗留 WinForms 版本)
```bash
cd MsPaint
# 使用 Visual Studio 或其他 .NET IDE 打开 MsPaint.csproj
```

## 使用方法 (WinForms 版本)
1. 运行程序
2. 点击"打开"菜单，选择TXT格式的心电图数据文件
3. 点击"开始"按钮开始实时显示
4. 点击"停止"按钮暂停显示
5. 在文本框中设置QT间期的正常范围（默认值需要根据实际情况调整）

## 界面说明
- **pictureBox1**: 主要的心电图显示区域
- **pictureBox2**: QT间期状态指示器（绿色表示正常，红色表示异常）
- **textBox1**: QT间期下限设置
- **textBox2**: 当前QT间期显示
- **textBox3**: 当前QR间期显示
- **textBox4**: 平均QT间期显示
- **textBox5**: QT间期上限设置

## 技术实现
- 使用C# WinForms开发
- 采用定时器实现实时显示
- 使用GDI+进行图形绘制
- 支持动态网格背景
- 实时计算心电图参数

## 项目结构
```
electrocardiogram-master/
├── ECGViewer.sln                    # 推荐的解决方案文件（包含新 WPF 项目）
├── ECGViewer.Wpf/                   # 现代 WPF 应用 (.NET 8)
│   ├── ECGViewer.Wpf.csproj
│   ├── App.xaml
│   ├── App.xaml.cs                  # DI 启动、全局异常处理
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── MsPaint/                         # 遗留 WinForms 应用
│   ├── Form1.cs                     # 主窗体代码
│   ├── Form1.Designer.cs            # 窗体设计器代码
│   ├── draw.cs                      # 绘图工具类
│   ├── Program.cs                   # 程序入口
│   └── 数据文件/
│       └── *.txt                    # 心电图数据文件
├── PaintRinhtNow.sln                # 遗留的 WinForms 解决方案
└── README.md                        # 本文件
```

## 架构设计

### ECGViewer.Wpf - 现代 WPF 实现

#### 依赖注入 (DI) 启动流程
- `App.xaml.cs` 在应用启动时使用 `Microsoft.Extensions.Hosting` 初始化主机
- 服务注册在 `ConfigureServices()` 方法中进行
- `MainWindow` 通过 DI 容器解析并显示

#### 全局异常处理
- `DispatcherUnhandledException` - 处理 UI 线程异常
- `AppDomain.CurrentDomain.UnhandledException` - 处理未捕获的异常
- `TaskScheduler.UnobservedTaskException` - 处理后台任务异常
- 所有异常通过用户友好的对话框呈现

#### 扩展性
项目设计时已预留了以下项目的条件引用（使用 `Condition="Exists(...)"` 保护）：
- `ECGViewer.Core` - 核心业务逻辑层
- `ECGViewer.Infrastructure` - 基础设施层（数据访问、IO 等）

这样做可以确保在这些层项目不存在时构建不会失败。

### 依赖包
- `Microsoft.Extensions.Hosting` (8.0.0) - 应用主机框架
- `Microsoft.Extensions.DependencyInjection` (8.0.0) - 依赖注入容器
- `WriteableBitmapEx` (1.6.8) - 高性能位图操作
- `CommunityToolkit.Mvvm` (8.2.2) - MVVM 支持库

## 注意事项
1. 确保TXT文件格式正确，每行一个数值（仅适用于 WinForms 版本）
2. 数据文件路径不能包含中文字符（仅适用于 WinForms 版本）
3. 程序会自动处理数据转换和显示（仅适用于 WinForms 版本）
4. 建议使用标准的心电图数据文件进行测试（仅适用于 WinForms 版本）

## 贡献指南
- 新功能请在 `ECGViewer.Wpf` 项目中实现
- 核心业务逻辑应提取到 `ECGViewer.Core` 中（如果存在）
- WinForms 版本仅用于遗留支持，不建议再进行重大开发