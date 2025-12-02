using System;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // ViewModel属性，用于数据绑定
        public ChartViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // 创建默认参数
            var chartParas = new ChartParas
            {
                XTimeSpan = 300,
                XStep = 1,
                YStep = 5,
                YMax = 100,
                YMin = 0
            };

            // 初始化ViewModel并设置数据上下文
            ViewModel = new ChartViewModel(chartParas);
            this.DataContext = ViewModel;

            // 配置LiveCharts的数据映射
            var mapper = Mappers.Xy<MeasureData>()
                .X(model => model.DateTime.Ticks)    // X轴映射到时间刻度
                .Y(model => model.Value);            // Y轴映射到数值

            // 全局保存映射器配置
            Charting.For<MeasureData>(mapper);

            // 注册窗口事件
            this.Loaded += Window_Loaded;
            this.Closing += Window_Closing;
        }

        #region 依赖属性
        public static readonly DependencyProperty MainBackGroundProperty =
            DependencyProperty.Register("MainBackGround", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 线条填充属性
        /// </summary>
        public Brush LineFill
        {
            get { return (Brush)GetValue(LineFillProperty); }
            set { SetValue(LineFillProperty, value); }
        }

        // LineFill依赖属性定义
        public static readonly DependencyProperty LineFillProperty =
            DependencyProperty.Register("LineFill", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.Transparent));

        // 线条颜色属性
        public Brush LineStroke
        {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        // LineStroke依赖属性定义
        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register("LineStroke", typeof(Brush), typeof(MainWindow),
                new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region 窗口事件处理
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalPara.IsShowChart = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalPara.IsShowChart = false;
        }
        #endregion
    }
}



