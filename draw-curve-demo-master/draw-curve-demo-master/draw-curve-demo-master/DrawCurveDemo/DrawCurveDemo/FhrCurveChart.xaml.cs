using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DrawCurveDemo
{
    /// <summary>
    /// FHR（胎心率）曲线图表控件
    /// </summary>
    public partial class FhrCurveChart : UserControl
    {
        private int clickIndex = 0;               // 点击类型：0=无点击,1=拖动滑块,2=拖动画布
        private double offset = -1;               // 鼠标位置偏移量，用于画布拖动计算
        private Point pointBorder;                // 滑块上的点击位置坐标
        private bool isScrollEnd = true;          // 是否滚动到底部（自动走纸模式）
        private DispatcherTimer scrollEndTimer;   // 滚动结束计时器（5秒后恢复自动走纸）

        public FhrCurveChart()
        {
            InitializeComponent();  // 初始化XAML组件
        }

        /// <summary>
        /// 添加实时胎心率数据
        /// </summary>
        /// <param name="data">胎心率数据数组</param>
        public void AddFhrData(FhrModel[] data)
        {
            // 如果数据为空则直接返回
            if (data == null || data.Length == 0) return;

            // 更新最后索引值（如果新数据的最后一个索引更大）
            if (!curveDraw.LastIndex.HasValue || data[^1].Index > curveDraw.LastIndex)
            {
                curveDraw.LastIndex = data[^1].Index;
            }

            // 将数据添加到字典中（如果字典中不存在该索引）
            for (int i = 0; i < data.Length; i++)
            {
                if (!curveDraw.DicFhrData.ContainsKey(data[i].Index.Value))
                {
                    curveDraw.DicFhrData[data[i].Index.Value] = data[i];
                }
            }

            // 更新界面显示的胎心率值
            txtFhr1.Text = data[^1].Fhr1 + "";
            txtFhr2.Text = data[^1].Fhr2 + "";
            txtFhr3.Text = data[^1].Fhr3 + "";

            // 根据数据量更新画布宽度
            canvas.Width = curveDraw.LastIndex.Value / 2 * curveDraw.Xscale;

            // 计算并更新滑块宽度（滑块宽度与可见区域/总数据量成比例）
            borderSlider.Width = curveDraw.RenderSize.Width * curveDraw.RenderSize.Width / canvas.Width;

            // 根据是否需要滚动决定滑块可见性
            borderSlider.Visibility = canvas.Width > curveDraw.RenderSize.Width ? Visibility.Visible : Visibility.Collapsed;

            // 如果处于自动走纸模式，滚动到最右侧
            if (isScrollEnd)
            {
                scrollViewer.ScrollToRightEnd();
                scrollViewer.UpdateLayout();
            }
        }

        /// <summary>
        /// 滚动视图滚动变化事件处理
        /// </summary>
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // 通知曲线绘制控件更新偏移量
            curveDraw.UpdateOffset(scrollViewer.HorizontalOffset);

            // 计算并设置滑块位置（保持与内容滚动同步）
            Canvas.SetLeft(borderSlider, scrollViewer.HorizontalOffset /
                ((canvas.ActualWidth - curveDraw.RenderSize.Width) / (curveDraw.RenderSize.Width - borderSlider.ActualWidth)));
        }

        /// <summary>
        /// 画布鼠标左键按下事件（准备拖动画布）
        /// </summary>
        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clickIndex = 2;  // 设置当前操作为画布拖动
        }

        /// <summary>
        /// 滑块鼠标左键按下事件（准备拖动滑块）
        /// </summary>
        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clickIndex = 1;  // 设置当前操作为滑块拖动
            pointBorder = e.GetPosition(borderSlider);  // 记录鼠标在滑块上的位置
        }

        /// <summary>
        /// 鼠标移动事件处理（实现拖拽功能）
        /// </summary>
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果鼠标左键按下且处于拖动状态
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // 滑块拖动处理
                if (clickIndex == 1)
                {
                    isScrollEnd = false;      // 进入手动滚动模式
                    scrollEndTimer?.Stop();   // 停止自动走纸计时器

                    // 获取鼠标在画布面板上的当前位置
                    Point point = e.GetPosition(canvasPanel);

                    // 计算并限制滑块位置边界
                    if (point.X - pointBorder.X <= 0)
                    {
                        scrollViewer.ScrollToHorizontalOffset(0);  // 滚动到最左侧
                    }
                    else if (point.X - pointBorder.X >= canvasPanel.ActualWidth - borderSlider.ActualWidth)
                    {
                        // 滚动到最右侧
                        scrollViewer.ScrollToHorizontalOffset(canvas.ActualWidth - canvasPanel.ActualWidth);
                    }
                    else if (point.X - pointBorder.X > 0 && point.X - pointBorder.X < canvasPanel.ActualWidth - borderSlider.ActualWidth)
                    {
                        // 根据鼠标位置计算并设置滚动位置
                        var left = point.X - pointBorder.X;
                        scrollViewer.ScrollToHorizontalOffset((canvas.ActualWidth - canvasPanel.ActualWidth) /
                            (canvasPanel.ActualWidth - borderSlider.ActualWidth) * left);
                    }
                }

                // 画布拖动处理
                if (clickIndex == 2)
                {
                    isScrollEnd = false;      // 进入手动滚动模式
                    scrollEndTimer?.Stop();   // 停止自动走纸计时器

                    // 计算并设置基于鼠标移动的滚动位置
                    if (offset >= 0 && offset <= canvasPanel.ActualWidth)
                    {
                        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - (e.GetPosition(this).X - offset));
                    }
                    offset = e.GetPosition(this).X;  // 更新当前鼠标位置
                }
            }
            else
            {
                // 如果拖动结束（鼠标释放），处理拖动结束逻辑
                if (clickIndex != 0) DoDragCurveEnd();

                // 重置状态
                clickIndex = 0;
                offset = -1;
            }
        }

        /// <summary>
        /// 鼠标离开画布事件
        /// </summary>
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            // 如果拖动状态中鼠标离开，处理结束逻辑
            if (clickIndex != 0) DoDragCurveEnd();
        }

        /// <summary>
        /// 鼠标按键释放事件
        /// </summary>
        private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            DoDragCurveEnd();  // 处理拖动结束逻辑
        }

        /// <summary>
        /// 拖动结束处理（5秒后恢复自动走纸）
        /// </summary>
        private void DoDragCurveEnd()
        {
            // 如果滚动到最右侧（差值小于10像素），立即恢复自动走纸
            if (scrollViewer.HorizontalOffset >= scrollViewer.ScrollableWidth - 10)
            {
                scrollEndTimer?.Stop();
                isScrollEnd = true;
            }
            else
            {
                // 否则启动5秒计时器，计时结束后恢复自动走纸
                scrollEndTimer?.Stop();
                scrollEndTimer = new()
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                scrollEndTimer.Tick += (s, e) =>
                {
                    isScrollEnd = true;      // 恢复自动走纸模式
                    scrollEndTimer?.Stop();  // 停止计时器
                };
                scrollEndTimer.Start();
            }
        }
    }
}

//│   │   ├── FhrCurveChart/                       # 胎心率曲线图表控件
//│   │   │   ├── 基础字段区
//│   │   │   │   ├── 点击类型标记 (_clickIndex)              # 记录当前操作类型（0=无/1=滑块拖动/2=画布拖动）
//│   │   │   │   ├── 鼠标偏移量 (_offset)                   # 记录画布拖动时的初始鼠标位置
//│   │   │   │   ├── 滑块点击坐标 (_pointBorder)            # 存储滑块上的点击起始位置
//│   │   │   │   ├── 自动滚动标记 (_isScrollEnd)            # 是否开启自动滚动到末尾的标记
//│   │   │   │   ├── 滚动恢复计时器 (_scrollEndTimer)       # 5秒后恢复自动滚动的计时器
//│   │
//│   │   │   ├── 核心方法区
//│   │   │   │   ├── 构造函数
//│   │   │   │   │   ├── InitializeComponent()             # 初始化XAML组件
//│   │   │   │   │
//│   │   │   │   ├── 数据操作
//│   │   │   │   │   ├── AddFhrData()                      # 添加胎心率数据（主业务逻辑）
//│   │   │   │   │   │   ├── 更新最后索引值                 # 动态扩展X轴范围
//│   │   │   │   │   │   ├── 数据字典去重存储               # 使用DicFhrData避免重复点
//│   │   │   │   │   │   ├── UI数值显示更新                 # 更新txtFhr1/2/3文本框
//│   │   │   │   │   │   ├── 画布动态伸缩逻辑               # 根据数据量调整canvas.Width
//│   │   │   │   │   │   ├── 滑块显隐控制                   # 数据超长时显示滚动条
//│   │   │   │   │   │   └── 自动滚动判断                  # _isScrollEnd为true时滚到最右
//│   │
//│   │   │   │   ├── 交互逻辑
//│   │   │   │   │   ├── ScrollViewer_ScrollChanged()      # 滚动同步逻辑
//│   │   │   │   │   │   ├── 曲线重绘通知                  # 调用curveDraw.UpdateOffset
//│   │   │   │   │   │   └── 滑块位置计算                  # 保持滑块与内容滚动比例同步
//│   │   │   │   │   │
//│   │   │   │   │   ├── 鼠标事件组
//│   │   │   │   │   │   ├── Border_PreviewMouseLeftButtonDown()  # 滑块拖动开始
//│   │   │   │   │   │   ├── Canvas_PreviewMouseLeftButtonDown()  # 画布拖动开始
//│   │   │   │   │   │   ├── Canvas_MouseMove()            # 拖动过程处理（核心）
//│   │   │   │   │   │   │   ├── 滑块拖动模式               # 根据鼠标位移计算滚动位置
//│   │   │   │   │   │   │   ├── 画布拖动模式               # 基于鼠标位移的惯性滚动
//│   │   │   │   │   │   │   └── 拖动状态清理               # 鼠标释放后重置状态
//│   │   │   │   │   │   │
//│   │   │   │   │   │   ├── Canvas_MouseLeave()           # 意外中断处理
//│   │   │   │   │   │   └── Canvas_PreviewMouseUp()       # 拖动结束确认
//│   │   │   │   │   │
//│   │   │   │   │   └── DoDragCurveEnd()                  # 拖动终止后的处理
//│   │   │   │   │       ├── 立即恢复判断                  # 滚动位置接近末尾时直接恢复
//│   │   │   │   │       └── 延迟恢复机制                  # 启动5秒计时器恢复自动滚动
