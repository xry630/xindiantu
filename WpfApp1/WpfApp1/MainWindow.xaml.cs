

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    // MainWindow类，继承自Window，是应用程序的主窗口
    public partial class MainWindow : Window
    {
        // ViewModel属性，用于数据绑定，绑定到视图的ChartViewModel实例
        public ChartViewModel ViewModel { get; private set; }

        // 默认构造函数
        public MainWindow()
        {
            InitializeComponent();  // 初始化组件（UI元素）

            // 在没有参数的情况下使用默认值初始化 ChartParas
            ChartParas para = new ChartParas
            {
                // 根据需要填充ChartParas的属性
            };

            // 初始化ChartViewModel，并将其作为数据上下文绑定到窗口
            ViewModel = new ChartViewModel(para);
            this.DataContext = ViewModel;  // 设置数据上下文，使得UI能绑定ViewModel中的数据

            // 配置LiveCharts的数据映射，将MeasureData类型的X轴映射为时间刻度，Y轴映射为数值
            var mapper = Mappers.Xy<MeasureData>()
                .X(model => model.DateTime.Ticks)    // 将MeasureData的DateTime属性映射到X轴，单位为时间的刻度
                .Y(model => model.Value);            // 将MeasureData的Value属性映射到Y轴
            // 全局注册数据映射配置，用于后续的LiveCharts图表显示
            Charting.For<MeasureData>(mapper);

            // 注册窗口事件
            this.Loaded += Window_Loaded;
            this.Closing += Window_Closing;
        }

        #region 依赖属性
        // 声明MainBackGround依赖属性，用于设置窗口背景颜色
        public static readonly DependencyProperty MainBackGroundProperty =
            DependencyProperty.Register("MainBackGround", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.Transparent));

        // LineFill属性，用于设置图表线条的填充颜色
        public Brush LineFill
        {
            get { return (Brush)GetValue(LineFillProperty); }  // 获取LineFill的值
            set { SetValue(LineFillProperty, value); }         // 设置LineFill的值
        }

        // LineFill依赖属性，用于设置图表线条的填充颜色
        public static readonly DependencyProperty LineFillProperty =
            DependencyProperty.Register("LineFill", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.Transparent));

        // LineStroke属性，用于设置图表线条的描边颜色
        public Brush LineStroke
        {
            get { return (Brush)GetValue(LineStrokeProperty); }  // 获取LineStroke的值
            set { SetValue(LineStrokeProperty, value); }         // 设置LineStroke的值
        }

        // LineStroke依赖属性，用于设置图表线条的描边颜色
        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register("LineStroke", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region 窗口事件处理
        // 窗口加载事件处理方法
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置全局参数，标记图表可见
            GlobalPara.IsShowChart = true;
        }

        // 窗口关闭事件处理方法
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 设置全局参数，标记图表不可见
            GlobalPara.IsShowChart = false;
        }

        // 添加事件处理方法：用于处理 Chart1 的 Loaded 事件
        private void Chart1_Loaded(object sender, RoutedEventArgs e)
        {
            // 在图表加载完成后执行的操作
            Console.WriteLine("Chart1 Loaded");
        }

        // 鼠标滚轮事件处理，用于缩放图表
        private void Chart_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 根据滚轮方向放大或缩小时间范围
            ViewModel.AdjustTimeRange(e.Delta > 0);
        }

        // 鼠标移动事件处理，用于拖动图表
        private void Chart_OnMouseMove(object sender, MouseEventArgs e)
        {
            // 如果按下鼠标左键，则处理拖动
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.DragChart(e.GetPosition(this).X);
            }
        }

        // 鼠标按下事件处理，用于初始化鼠标位置
        private void Chart_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.InitializeMousePosition(e.GetPosition(this).X);
            }
        }
        #endregion
    }
}

//│   │   ├── MainWindow.xaml.cs               # 主窗口类
//│   │   │   ├── ViewModel 属性             # 数据绑定属性，用于绑定ChartViewModel实例
//│   │   │   ├── 构造函数                   # 初始化窗口和视图模型，配置图表
//│   │   │   │   ├── 初始化组件             # 初始化UI组件（控件）
//│   │   │   │   ├── 创建默认参数           # 设置图表的初始参数（X轴、Y轴的范围与步长）
//│   │   │   │   ├── 初始化ViewModel         # 初始化ChartViewModel，并设置数据上下文
//│   │   │   │   ├── 配置LiveCharts的数据映射 # 配置LiveCharts的X和Y轴映射
//│   │   │   │   ├── 注册窗口事件           # 注册窗口加载与关闭事件
//│   │   │   ├── 依赖属性                   # 定义用于绑定UI控件的依赖属性
//│   │   │   │   ├── MainBackGroundProperty  # 设置窗口背景颜色的依赖属性
//│   │   │   │   ├── LineFillProperty        # 设置图表线条填充颜色的依赖属性
//│   │   │   │   ├── LineStrokeProperty      # 设置图表线条描边颜色的依赖属性
//│   │   │   ├── 窗口事件处理方法           # 处理窗口加载与关闭事件的方法
//│   │   │   │   ├── Window_Loaded          # 窗口加载时的处理方法（显示图表）
//│   │   │   │   ├── Window_Closing         # 窗口关闭时的处理方法（隐藏图表）