using Microsoft.Win32; // 引入文件操作相关的命名空间
using System; // 引入基础类库
using System.Collections.Generic; // 引入集合类命名空间
using System.Linq; // 引入LINQ命名空间
using System.Threading.Tasks; // 引入异步编程相关的命名空间
using System.Windows; // 引入WPF相关的命名空间
using System.Windows.Threading; // 引入调度器相关的命名空间
using LiveCharts; // 引入图表绘制库LiveCharts
using WpfApp1.Base; // 引入项目的基础库
using WpfApp1.Models; // 引入项目中的模型类
using WpfApp1.Util; // 引入工具类
using System.Windows.Input; // 引入命令和输入的命名空间

namespace WpfApp1.ViewModels // 定义视图模型命名空间
{
    // 图表视图模型类，继承自NotificationObject
    public class ChartViewModel : NotificationObject
    {
        #region 字段定义部分

        // 格式化日期时间的委托
        public Func<double, string> DateTimeFormatter { get; set; }

        // 用于临时存储测量数据
        private ChartValues<MeasureData> _tempValues = new ChartValues<MeasureData>();

        // 用于存储图表数据
        private ChartValues<MeasureData> _ChartValues = new ChartValues<MeasureData>();

        // 加载txt数据的命令
        public ICommand LoadTxtDataCommand { get; private set; }

        // 定义数据计时器
        private DispatcherTimer _dataTimer = new DispatcherTimer();

        // 存储所有测量数据
        private List<MeasureData> _measureDataList = new List<MeasureData>();

        // 当前数据索引
        private int _currentIndex = 0;

        // 上一次鼠标X坐标
        private double _lastMouseX;

        // 坐标轴单位，默认单位为1分钟
        private double _AxisUnit = TimeSpan.FromMinutes(1).Ticks;

        #endregion

        #region 属性定义部分

        // 坐标轴单位属性
        public double AxisUnit
        {
            get { return _AxisUnit; }
            set
            {
                _AxisUnit = value;
                RaisePropertyChanged("AxisUnit"); // 通知属性变更
            }
        }

        // 图表数据属性
        public ChartValues<MeasureData> ChartValues
        {
            get => _ChartValues;
            set
            {
                _ChartValues = value;
                RaisePropertyChanged(); // 通知属性变更
            }
        }

        // 当前值属性
        private double _CurValue;
        public double CurValue
        {
            get => _CurValue;
            set
            {
                _CurValue = value;
                RaisePropertyChanged(); // 通知属性变更
            }
        }

        // X轴时间跨度属性（用于调整时间范围）
        public int XTimeSpan { get; set; } = 300; // 默认时间跨度为300秒

        // X轴步长属性
        private double _XAxisStep = TimeSpan.FromMinutes(1).Ticks;
        public double XAxisStep
        {
            get { return _XAxisStep; }
            set
            {
                _XAxisStep = value;
                RaisePropertyChanged(); // 通知属性变更
            }
        }

        // Y轴步长属性
        private double _YAxisStep = 30;
        public double YAxisStep
        {
            get { return _YAxisStep; }
            set
            {
                _YAxisStep = value;
                RaisePropertyChanged(); // 通知属性变更
            }
        }

        // Y轴最小值属性
        private double _YAxisMin = 0;
        public double YAxisMin
        {
            get { return _YAxisMin; }
            set
            {
                _YAxisMin = value;
                RaisePropertyChanged(); // 通知属性变更
            }
        }

        // Y轴最大值属性
        private double _YAxisMax = 100;
        public double YAxisMax
        {
            get { return _YAxisMax; }
            set
            {
                _YAxisMax = value;
                RaisePropertyChanged(); // 通知属性变更
            }
        }

        // X轴最大值属性
        private double _axisMax1;
        public double AxisMax1
        {
            get { return _axisMax1; }
            set
            {
                _axisMax1 = value;
                RaisePropertyChanged("AxisMax1"); // 通知属性变更
            }
        }

        // X轴最小值属性
        private double _axisMin1;
        public double AxisMin1
        {
            get { return _axisMin1; }
            set
            {
                _axisMin1 = value;
                RaisePropertyChanged("AxisMin1"); // 通知属性变更
            }
        }

        #endregion

        #region 构造函数部分

        // 构造函数，初始化图表视图模型并设置相关参数
        public ChartViewModel(ChartParas para)
        {
            try
            {
                // 设置加载数据的命令
                LoadTxtDataCommand = new DelegateCommand(LoadTxtData);

                // 格式化日期时间
                DateTimeFormatter = value => new DateTime((long)value).ToString("HH:mm");

                // 设置X轴时间跨度和步长
                XTimeSpan = para.XTimeSpan;
                XAxisStep = TimeSpan.FromSeconds(para.XStep).Ticks;

                // 设置Y轴步长、最大值和最小值
                YAxisStep = para.YStep;
                YAxisMax = para.YMax;
                YAxisMin = para.YMin;

                // 设置定时器间隔为500毫秒
                _dataTimer.Interval = TimeSpan.FromMilliseconds(500);
                _dataTimer.Tick += DataTimer_Tick; // 定时器每次触发时执行DataTimer_Tick方法
            }
            catch (Exception ex)
            {
                FileTool.SaveFailLog(ex.Message); // 记录异常日志
            }

            // 设置坐标轴限制
            SetAxisLimits(DateTime.Now);
        }

        #endregion

        #region 数据加载和更新部分

        // 异步加载txt数据
        private async void LoadTxtData()
        {
            try
            {
                // 打开文件选择对话框
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*", // 文件过滤器
                    Title = "选择数据文件" // 文件选择对话框标题
                };

                if (openFileDialog.ShowDialog() == true) // 如果选择了文件
                {
                    var data = await Task.Run(() => TxtFileReader.ReadTxtFile(openFileDialog.FileName)); // 异步读取txt文件

                    if (data != null && data.Count > 0) // 如果数据有效
                    {
                        _measureDataList.Clear(); // 清空原数据
                        _measureDataList.AddRange(data); // 添加新数据

                        _currentIndex = 0; // 重置索引

                        // 将数据添加到图表并延时1秒
                        foreach (var measureData in _measureDataList)
                        {
                            AddData(measureData.Value);
                            await Task.Delay(1000);
                        }

                        _dataTimer.Start(); // 启动数据定时器
                    }
                }
            }
            catch (Exception ex)
            {
                FileTool.SaveFailLog(ex.Message); // 记录异常日志
            }
        }

        // 定时器触发时执行的方法
        private void DataTimer_Tick(object sender, EventArgs e)
        {
            if (_currentIndex < _measureDataList.Count) // 如果当前索引小于数据列表的长度
            {
                var data = _measureDataList[_currentIndex]; // 获取当前数据
                AddData(data.Value); // 将数据添加到图表
                _currentIndex++; // 索引递增
            }
            else
            {
                _dataTimer.Stop(); // 停止定时器
            }
        }

        // 将数据添加到图表
        private void AddData(double value)
        {
            try
            {
                CurValue = value; // 更新当前值
                var measureData = new MeasureData
                {
                    DateTime = DateTime.Now, // 获取当前时间
                    Value = value // 设置测量值
                };

                _tempValues.Add(measureData); // 将数据添加到临时列表

                // 在主线程更新图表
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ChartValues.Count >= 100) // 如果图表数据点超过100
                    {
                        ChartValues.RemoveAt(0); // 移除最早的数据点
                        UpdateAxisLimits(); // 更新坐标轴范围
                    }

                    ChartValues.AddRange(_tempValues); // 将临时数据添加到图表
                    _tempValues.Clear(); // 清空临时数据

                    UpdateYAxisLimits(value); // 动态更新Y轴的最小值和最大值
                });
            }
            catch (Exception ex)
            {
                FileTool.SaveFailLog(ex.Message); // 记录异常日志
            }
        }

        #endregion

        #region 坐标轴更新部分

        // 设置坐标轴的限制值
        private void SetAxisLimits(DateTime now)
        {
            int start = 20;  // 偏移量开始位置
            int stop = 120;  // 偏移量结束位置
            AxisMax1 = now.Ticks + TimeSpan.FromSeconds(stop).Ticks;  // 设置X轴最大值
            AxisMin1 = now.Ticks - TimeSpan.FromSeconds(start).Ticks;  // 设置X轴最小值
        }

        // 更新坐标轴的限制
        private void UpdateAxisLimits()
        {
            if (ChartValues.Count > 0) // 如果图表数据不为空
            {
                var currentTime = DateTime.Now; // 获取当前时间

                _tempValues.Clear(); // 清空临时存储的数据

                // 遍历图表数据，保留过去10秒的数据
                foreach (var data in ChartValues)
                {
                    if ((currentTime - data.DateTime).TotalSeconds <= 10)
                    {
                        _tempValues.Add(data);
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChartValues.Clear(); // 清空图表数据
                    ChartValues.AddRange(_tempValues); // 将过滤后的数据添加到图表
                    _tempValues.Clear(); // 清空临时列表
                });

                SetAxisLimits(currentTime); // 更新坐标轴限制
            }
        }

        // 更新Y轴的最大最小值
        private void UpdateYAxisLimits(double newValue)
        {
            if (newValue > YAxisMax) // 如果新的值大于当前Y轴最大值
            {
                YAxisMax = newValue; // 更新Y轴最大值
            }

            if (newValue < YAxisMin) // 如果新的值小于当前Y轴最小值
            {
                YAxisMin = newValue; // 更新Y轴最小值
            }
        }

        #endregion

        #region 交互部分

        // 调整时间范围（缩放图表）
        public void AdjustTimeRange(bool zoomIn)
        {
            if (zoomIn)
            {
                XTimeSpan = Math.Max(20, XTimeSpan - 10); // 最小时间跨度为20秒
            }
            else
            {
                XTimeSpan = Math.Min(600, XTimeSpan + 10); // 最大时间跨度为600秒
            }
            SetAxisLimits(DateTime.Now); // 更新坐标轴限制
        }

        // 初始化鼠标位置
        public void InitializeMousePosition(double mouseX)
        {
            _lastMouseX = mouseX; // 记录鼠标位置
        }

        // 拖动图表
        public void DragChart(double currentMouseX)
        {
            double delta = currentMouseX - _lastMouseX; // 计算鼠标移动的距离
            AxisMin1 += delta; // 更新坐标轴的最小值
            AxisMax1 += delta; // 更新坐标轴的最大值
            _lastMouseX = currentMouseX; // 更新最后的鼠标位置
        }

        #endregion
    }
}
//给出首次创建图表，图表更新，以及时间戳更新的规则

//│   │   ├── ViewModels /                     # 视图模型类
//│   │   │   ├── ChartViewModel              # 图表视图模型类
//│   │   │   │   ├── 属性字段              # 存储数据和配置信息的字段
//│   │   │   │   │   ├── _tempValues        # 用于临时存储测量数据
//│   │   │   │   │   ├── _ChartValues       # 用于存储图表数据
//│   │   │   │   │   ├── _measureDataList  # 存储所有测量数据
//│   │   │   │   │   ├── _currentIndex     # 当前数据索引
//│   │   │   │   │   ├── _lastMouseX       # 上一次鼠标X坐标
//│   │   │   │   │   ├── _dataTimer        # 数据计时器
//│   │   │   │   │   ├── _AxisUnit         # 坐标轴单位
//│   │   │   │   │   ├── _axisMax1         # X轴最大值
//│   │   │   │   │   ├── _axisMin1         # X轴最小值
//│   │   │   │   │   ├── _XAxisStep        # X轴步长
//│   │   │   │   │   ├── _YAxisStep        # Y轴步长
//│   │   │   │   │   ├── _YAxisMin         # Y轴最小值
//│   │   │   │   │   ├── _YAxisMax         # Y轴最大值
//│   │   │   │   ├── 属性                 # 属性，提供给视图绑定和外部访问
//│   │   │   │   │   ├── DateTimeFormatter  # 格式化日期时间的委托
//│   │   │   │   │   ├── LoadTxtDataCommand # 加载txt数据的命令
//│   │   │   │   │   ├── AxisUnit          # 坐标轴单位
//│   │   │   │   │   ├── AxisMax1          # X轴最大值
//│   │   │   │   │   ├── AxisMin1          # X轴最小值
//│   │   │   │   │   ├── CurValue          # 当前值
//│   │   │   │   │   ├── ChartValues       # 图表数据
//│   │   │   │   │   ├── XTimeSpan         # X轴时间跨度
//│   │   │   │   │   ├── XAxisStep         # X轴步长
//│   │   │   │   │   ├── YAxisStep         # Y轴步长
//│   │   │   │   │   ├── YAxisMin          # Y轴最小值
//│   │   │   │   │   ├── YAxisMax          # Y轴最大值
//│   │   │   │   ├── 构造函数             # 初始化视图模型并设置相关参数
//│   │   │   │   ├── 方法                  # 方法，处理数据更新、交互等操作
//│   │   │   │   │   ├── LoadTxtData       # 异步加载txt数据
//│   │   │   │   │   ├── DataTimer_Tick    # 定时器触发时执行的方法
//│   │   │   │   │   ├── SetAxisLimits     # 设置坐标轴的限制值
//│   │   │   │   │   ├── AddData           # 将数据添加到图表
//│   │   │   │   │   ├── UpdateAxisLimits  # 更新坐标轴的限制
//│   │   │   │   │   ├── UpdateYAxisLimits # 更新Y轴的最大最小值
//│   │   │   │   │   ├── AdjustTimeRange   # 调整时间范围（缩放图表）
//│   │   │   │   │   ├── InitializeMousePosition # 初始化鼠标位置
//│   │   │   │   │   ├── DragChart         # 拖动图表
//│   │   │   │   ├── 事件和命令            # 事件处理与命令执行
//│   │   │   │   │   ├── LoadTxtDataCommand # 加载数据的命令
//│   │   │   │   │   ├── _dataTimer        # 定时器相关事件




