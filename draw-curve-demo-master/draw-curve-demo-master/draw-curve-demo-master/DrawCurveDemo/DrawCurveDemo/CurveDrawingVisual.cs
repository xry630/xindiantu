using DrawCurveDemo;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace DrawCurveDemo
{
    /// <summary>
    /// 曲线绘制可视化组件，继承自FrameworkElement
    /// </summary>
    public class CurveDrawingVisual : FrameworkElement
    {
        private readonly DrawingVisual drawingVisual; // 用于绘制的Visual对象
        private readonly VisualCollection visuals; // 可视化元素集合
        private readonly double curveThickness = 1.5; // 曲线线宽
        private readonly double primaryThickness = 1.2; // 主网格线宽
        private readonly double secondaryThickness = 0.8; // 次网格线宽
        private int maxVal = 2500; // 心率最大值
        private int minVal = 1000; // 心率最小值
        private int currentMaxVal = 2500; // 当前实际最大值
        private int currentMinVal = 1000; // 当前实际最小值
        private readonly int topVal = 10; // 顶部留白
        private readonly int bottomVal = 25; // 底部留白
        private readonly int timeGap = 2; // 参考时间间隔(分钟)
        private readonly int xGap = 20; // 竖线间隔(像素)
        private readonly int fhr2Offset = -20; // 心率曲线2垂直偏移
        private readonly int fhr3Offset = 20; // 心率曲线3垂直偏移
        private double offset; // 水平偏移值
        private readonly int direction = 0; // 走纸方向(0:从右开始,1:从左开始)

        // 颜色定义
        public string fhr1Brush = "#009C26"; // 曲线1颜色(绿色)
        public string fhr2Brush = "#8080C0"; // 曲线2颜色(紫色)
        public string fhr3Brush = "#000000"; // 曲线3颜色(黑色)
        public string primaryBrush = "#DADADA"; // 主网格线颜色(浅灰色)
        public string secondaryBrush = "#E8E8E8"; // 次网格线颜色(更浅的灰色)

        private Pen fhr1Pen; // 曲线1画笔
        private Pen fhr2Pen; // 曲线2画笔
        private Pen fhr3Pen; // 曲线3画笔
        private Pen primaryPen; // 主网格线画笔
        private Pen secondaryPen; // 次网格线画笔
        private SolidColorBrush gridBack; // 背景色画刷

        public double Xscale { get; set; } = 1; // 横向缩放比例
        public Dictionary<int, FhrModel> DicFhrData { get; set; } = new(); // 心率数据字典
        public int? LastIndex { get; set; } // 最新时间索引
        public DateTime? StartTime { get; set; } = DateTime.Now; // 开始时间

        /// <summary>
        /// 构造函数
        /// </summary>
        public CurveDrawingVisual()
        {
            drawingVisual = new DrawingVisual(); // 创建绘图Visual
            visuals = new VisualCollection(this) { drawingVisual }; // 初始化可视化集合
            Init(); // 初始化组件
            InvalidateVisual(); // 触发重绘
        }

        /// <summary>
        /// 初始化方法，设置画笔和颜色
        /// </summary>
        private void Init()
        {
            gridBack = Brushes.White; // 设置白色背景

            // 创建各种画笔并设置颜色和线宽
            fhr1Pen = new Pen(BrushUtil.ToBrush(fhr1Brush), curveThickness);
            fhr2Pen = new Pen(BrushUtil.ToBrush(fhr2Brush), curveThickness);
            fhr3Pen = new Pen(BrushUtil.ToBrush(fhr3Brush), curveThickness);
            primaryPen = new Pen(BrushUtil.ToBrush(primaryBrush), primaryThickness);
            secondaryPen = new Pen(BrushUtil.ToBrush(secondaryBrush), secondaryThickness);

            // 冻结画笔以提高性能(冻结后不可修改)
            fhr1Pen.Freeze();
            fhr2Pen.Freeze();
            fhr3Pen.Freeze();
            primaryPen.Freeze();
            secondaryPen.Freeze();
        }

        /// <summary>
        /// 更新水平偏移值
        /// </summary>
        /// <param name="offset">新的偏移值</param>
        public void UpdateOffset(double offset)
        {
            this.offset = offset;
            InvalidateVisual(); // 触发重绘
        }

        /// <summary>
        /// 绘制内容的主要方法
        /// </summary>
        /// <param name="dc">绘图上下文</param>
        private void DrawContent(DrawingContext dc)
        {
            if (RenderSize.Height <= 0 || RenderSize.Width <= 0) return;

            var height = RenderSize.Height; // 获取控件实际高度
            var width = RenderSize.Width; // 获取控件实际宽度
            
            // 确保最大最小值之间有合理的差值
            if (maxVal <= minVal) maxVal = minVal + 50;
            
            var y_scale = (height - bottomVal - topVal) / (maxVal - minVal); // 计算Y轴缩放比例

            // 1. 绘制底色
            dc.DrawRectangle(gridBack, null, new Rect(0, 0, width, height));

            Point point1;
            Point point2;

            // 2. 绘制横线(网格线)
            for (int y = minVal; y <= maxVal; y += 10)
            {
                // 计算当前Y坐标位置
                var h = height - (y - minVal) * y_scale - bottomVal;
                point1 = new Point(0, h);
                point2 = new Point(width, h);

                // 每30单位绘制主网格线，其他绘制次网格线
                if (y % 50 == 0)
                {
                    dc.DrawLine(primaryPen, point1, point2);
                    continue;
                }
                dc.DrawLine(secondaryPen, point1, point2);
            }

            // 3. 计算偏移量和缩放
            var x_offset = offset / Xscale; // 曲线零点与绘图零点偏移量
            var origin_offset = x_offset;
            var width_scale = width / Xscale;

            // 处理曲线长度小于视图宽度时的偏移量
            if (LastIndex.HasValue && width_scale > LastIndex.Value / 2)
            {
                if (direction == 0) // 从右开始绘制
                {
                    origin_offset = (float)width_scale - LastIndex.Value / 2;
                    x_offset = 120 * timeGap - origin_offset % (120 * timeGap); // 计算时间区间偏移量
                    origin_offset = -origin_offset;
                }
                else // 从左开始绘制
                {
                    x_offset = 0;
                    origin_offset = 0;
                }
            }

            // 禁用过期API警告(FormattedText在.NET Core中有新API)
#pragma warning disable CS0618

            // 4. 绘制竖线与纵坐标刻度文字
            var max_index = width_scale + x_offset;
            for (int i = 0; i < max_index; i += xGap * 2)
            {
                if (i < x_offset) continue;
                var w = (i - x_offset) * Xscale;

                // 每2分钟大刻度线时绘制胎心刻度值
                if ((i + (60 * timeGap)) % (60 * timeGap * 2) == 0)
                {
                    // 绘制胎心刻度值(30单位间隔)
                    for (int y = minVal; y <= maxVal; y += 100)
                    {
                        // 创建格式化文本
                        var text = new FormattedText(y + "", CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight, new Typeface("Verdana"), 9, Brushes.Black);
                        // 绘制文本
                        dc.DrawText(text, new Point(w, height - (y - minVal) * y_scale - bottomVal - text.Height));
                    }
                }

                // 绘制竖线
                point1 = new Point(w, topVal);
                point2 = new Point(w, height - bottomVal);

                // 每4分钟(120点)绘制主网格线，其他绘制次网格线
                if (i % 120 == 0)
                {
                    dc.DrawLine(primaryPen, point1, point2);
                    continue;
                }
                dc.DrawLine(secondaryPen, point1, point2);
            }

            // 5. 计算绘制范围
            var start = origin_offset < 0 ? 0 : (int)origin_offset;
            var maxIndex = start * 2 + width_scale * 2; // 可视区域最多绘制点数

            // 6. 绘制横坐标时间文字
            for (int i = start * 2, left = start; i <= maxIndex && i <= LastIndex; i += 2, left++)
            {
                if (i % 4 == 0) // 每分钟一次
                {
                    if (StartTime.HasValue)
                    {
                        int seconds = i / 4;
                        if (seconds % (60 * timeGap) == 0) // 每分钟检查一次
                        {
                            // 每2分钟显示完整时间，每分钟显示"分"
                            string time = seconds % (120 * timeGap) == 0 ?
                                StartTime.Value.AddSeconds(seconds).ToString("HH:mm") : $"{seconds / 60}分";

                            // 创建格式化文本
                            var text = new FormattedText(time, CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight, new Typeface("Verdana"), 11, Brushes.Black);

                            // 调整文本位置
                            if (seconds != 0)
                                dc.DrawText(text, new Point((left - origin_offset) * Xscale - text.Width / 2, height - bottomVal));
                            else
                                dc.DrawText(text, new Point((left - origin_offset) * Xscale, height - bottomVal));
                        }
                    }
                }
            }
#pragma warning restore CS0618 // 恢复过期API警告

            // 7. 绘制胎心率曲线
            FhrModel fhrModel_2 = null; // 前一个数据点
            for (int i = start * 2, left = start; i <= maxIndex && i <= LastIndex; i += 2, left++)
            {
                // 获取当前和上一个数据点
                DicFhrData.TryGetValue(i, out FhrModel fhrModel_1);
                if (fhrModel_1 != null && fhrModel_2 != null)
                {
                    var point1_x = (left - origin_offset) * Xscale;
                    var point2_x = (left - 1 - origin_offset) * Xscale;

                    // 绘制第一条胎心率曲线
                    if (CheckFhrData(fhrModel_1.Fhr1, fhrModel_2.Fhr1, 1))
                    {
                        point1 = new Point(point1_x, height - (fhrModel_1.Fhr1.Value - minVal) * y_scale - bottomVal);
                        point2 = new Point(point2_x, height - (fhrModel_2.Fhr1.Value - minVal) * y_scale - bottomVal);
                        dc.DrawLine(fhr1Pen, point1, point2);
                    }

                    // 绘制第二条胎心率曲线(带偏移)
                    if (CheckFhrData(fhrModel_1.Fhr2, fhrModel_2.Fhr2, 2))
                    {
                        point1 = new Point(point1_x, height - (fhrModel_1.Fhr2.Value + fhr2Offset - minVal) * y_scale - bottomVal);
                        point2 = new Point(point2_x, height - (fhrModel_2.Fhr2.Value + fhr2Offset - minVal) * y_scale - bottomVal);
                        dc.DrawLine(fhr2Pen, point1, point2);
                    }

                    // 绘制第三条胎心率曲线(带偏移) - 注意: 这里有笔误，应该是Fhr3而不是Fhr2
                    if (CheckFhrData(fhrModel_1.Fhr3, fhrModel_2.Fhr3, 3))
                    {
                        point1 = new Point(point1_x, height - (fhrModel_1.Fhr2.Value + fhr3Offset - minVal) * y_scale - bottomVal);
                        point2 = new Point(point2_x, height - (fhrModel_2.Fhr2.Value + fhr3Offset - minVal) * y_scale - bottomVal);
                        dc.DrawLine(fhr3Pen, point1, point2);
                    }
                }
                fhrModel_2 = fhrModel_1; // 更新前一个数据点
            }
        }

        /// <summary>
        /// 判断胎心率数据是否有效
        /// </summary>
        /// <param name="data1">当前数据点</param>
        /// <param name="data2">前一个数据点</param>
        /// <param name="fetal">胎儿编号(1-3)</param>
        /// <returns>是否有效</returns>
        private bool CheckFhrData(int? data1, int? data2, int fetal)
        {
            double offset = 0;
            if (fetal == 2) offset = fhr2Offset;
            if (fetal == 3) offset = fhr3Offset;

            if (data1.HasValue && data2.HasValue)
            {
                var value1 = data1.Value + offset;
                var value2 = data2.Value + offset;

                // 检查数据在有效范围内
                if (value1 >= 0 && value1 <= 3000 && value2 >= 0 && value2 <= 3000)
                {
                    // 检查相邻点差值是否合理
                    if (Math.Abs(data1.Value - data2.Value) < 400)
                    {
                        // 动态调整显示范围
                        UpdateDisplayRange(value1);
                        UpdateDisplayRange(value2);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 更新显示范围
        /// </summary>
        /// <param name="value">新的数据点值</param>
        private void UpdateDisplayRange(double value)
        {
            // 添加缓冲区，避免数据点太靠近边界
            const int buffer = 20;
            
            // 更新当前实际的最大最小值
            if (value < currentMinVal)
            {
                currentMinVal = (int)Math.Max(0, Math.Floor(value));
                // 确保最小值不会太接近0
                minVal = Math.Max(0, currentMinVal - buffer);
            }
            
            if (value > currentMaxVal)
            {
                currentMaxVal = (int)Math.Min(3000, Math.Ceiling(value));
                // 确保最大值不会超过300
                maxVal = Math.Min(3000, currentMaxVal + buffer);
            }

            // 如果范围太小，确保至少有最小显示范围
            const int minRange = 50;
            if (maxVal - minVal < minRange)
            {
                int midPoint = (maxVal + minVal) / 2;
                minVal = Math.Max(0, midPoint - minRange / 2);
                maxVal = Math.Min(30, midPoint + minRange / 2);
            }
        }

        // WPF可视化树相关方法

        /// <summary>
        /// 获取可视化子元素数量
        /// </summary>
        protected override int VisualChildrenCount => visuals.Count;

        /// <summary>
        /// 获取指定索引的可视化子元素
        /// </summary>
        protected override Visual GetVisualChild(int index) => visuals[index];

        /// <summary>
        /// 当渲染尺寸变化时触发
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            InvalidateVisual(); // 尺寸变化时重绘
            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// 渲染方法
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawContent(drawingContext); // 执行绘制
            base.OnRender(drawingContext);
        }
    }
}

//├── CurveDrawingVisual /                      # 胎心率曲线绘制控件
//│   ├── 基础字段 /                            # 基础配置字段
//│   │   ├── 绘图相关(_drawingVisual, _visuals)             # WPF绘图基础对象
//│   │   ├── 线条配置(_curveThickness, _primaryThickness)   # 各类线条粗细配置
//│   │   ├── 坐标轴配置(_maxVal, _minVal, _topVal)          # 坐标系范围参数  
//│   │   ├── 布局参数(_xGap, _fhr2Offset, _direction)       # 网格与曲线布局参数
//│   │   └── 颜色配置(fhr1Brush, primaryBrush)              # 调色板配置
//│   ├── 核心方法 /                            # 主要功能实现
//│   │   ├── 初始化方法(Init)                # 创建并冻结画笔对象
//│   │   ├── 绘图主逻辑(DrawContent)         # 含网格/曲线/坐标轴绘制
//│   │   │   ├── 坐标系计算(y_scale, x_offset)              # 动态计算缩放比例
//│   │   │   ├── 背景网格绘制                 # 主次横线/竖线绘制
//│   │   │   ├── 坐标标签绘制                 # 心率刻度/时间标签
//│   │   │   └── 三曲线绘制                  # 分段绘制胎心曲线
//│   │   └── 数据校验 (CheckFhrData)          # 曲线数据有效性检查
//│   ├── 交互控制/                            # 用户交互相关
//│   │   ├── 偏移量控制 (UpdateOffset)        # 处理水平滚动
//│   │   └── 缩放控制 (Xscale属性)            # 横向缩放比例
//│   └── 框架重写/                            # WPF框架方法
//│       ├── 可视化树方法 (VisualChildrenCount)              # Visual子元素管理
//│       └── 渲染控制 (OnRender, OnRenderSizeChanged)        # 绘制触发机制





//namespace DrawCurveDemo
//{
//    public class FhrChartControl : UserControl
//    {
//        private CartesianChart chart;
//        private LineSeries fhr1Series;


//        public ChartValues<double> Fhr1Values { get; set; }


//        public FhrChartControl()
//        {
//            InitializeChart();
//        }

//        private void InitializeChart()
//        {
//            // 初始化数据集合
//            Fhr1Values = new ChartValues<double>();

//            // 创建图表
//            chart = new CartesianChart
//            {
//                DisableAnimations = true,
//                AnimationsSpeed = TimeSpan.Zero,
//                Background = Brushes.White
//            };

//            // 配置X轴
//            chart.AxisX.Add(new Axis
//            {
//                MinValue = 0,
//                LabelFormatter = value => TimeSpan.FromSeconds(value * 15).ToString(@"mm\:ss"),
//                Separator = new Separator
//                {
//                    Step = 120, // 每2分钟一个主刻度
//                    StrokeThickness = 1,
//                    StrokeDashArray = null,
//                    Stroke = BrushUtil.ToBrush("#DADADA")
//                }
//            });

//            // 配置Y轴
//            chart.AxisY.Add(new Axis
//            {
//                MinValue = 30,
//                MaxValue = 240,
//                LabelFormatter = value => value.ToString("N0"),
//                Separator = new Separator
//                {
//                    Step = 30,
//                    StrokeThickness = 1,
//                    StrokeDashArray = null,
//                    Stroke = BrushUtil.ToBrush("#DADADA")
//                }
//            });

//            // 配置曲线系列
//            fhr1Series = new LineSeries
//            {
//                Values = Fhr1Values,
//                Fill = Brushes.Transparent,
//                Stroke = BrushUtil.ToBrush("#009C26"),
//                StrokeThickness = 1.5,
//                PointGeometry = null,
//                LineSmoothness = 0
//            };

//            fhr2Series = new LineSeries
//            {
//                Values = Fhr2Values,
//                Fill = Brushes.Transparent,
//                Stroke = BrushUtil.ToBrush("#8080C0"),
//                StrokeThickness = 1.5,
//                PointGeometry = null,
//                LineSmoothness = 0
//            };

//            fhr3Series = new LineSeries
//            {
//                Values = Fhr3Values,
//                Fill = Brushes.Transparent,
//                Stroke = BrushUtil.ToBrush("#000000"),
//                StrokeThickness = 1.5,
//                PointGeometry = null,
//                LineSmoothness = 0
//            };

//            // 添加曲线到图表
//            chart.Series = new SeriesCollection
//            {
//                fhr1Series,
//                fhr2Series,
//                fhr3Series
//            };

//            // 设置图表为控件内容
//            Content = chart;
//        }

//        /// <summary>
//        /// 添加新的胎心率数据
//        /// </summary>
//        public void AddFhrData(FhrModel[] data)
//        {
//            foreach (var point in data)
//            {
//                if (point.Fhr1.HasValue && point.Fhr1 >= 30 && point.Fhr1 <= 240)
//                    Fhr1Values.Add(point.Fhr1.Value);

//                if (point.Fhr2.HasValue && point.Fhr2 >= 30 && point.Fhr2 <= 240)
//                    Fhr2Values.Add(point.Fhr2.Value - 20); // 应用偏移

//                if (point.Fhr3.HasValue && point.Fhr3 >= 30 && point.Fhr3 <= 240)
//                    Fhr3Values.Add(point.Fhr3.Value + 20); // 应用偏移

//                // 保持固定数据点数量，避免内存占用过大
//                if (Fhr1Values.Count > 1440) // 保持6小时的数据
//                {
//                    Fhr1Values.RemoveAt(0);
//                }
//            }
//        }

//        /// <summary>
//        /// 清除所有数据
//        /// </summary>
//        public void ClearData()
//        {
//            Fhr1Values.Clear();
//        }
//    }
//}