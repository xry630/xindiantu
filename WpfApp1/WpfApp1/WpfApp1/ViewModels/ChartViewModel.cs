using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Util;
using LiveCharts;
using WpfApp1.Base;
using WpfApp1.Models;
using System.Windows.Forms;
using LiveCharts.Wpf;
using System.Windows.Input;
using System.Collections.ObjectModel;
using LiveCharts.Configurations;

namespace WpfApp1.ViewModels
{
    public class ChartViewModel : NotificationObject
    {
        #region 构造函数和初始化
        private readonly CartesianMapper<TxtDataModel> _txtDataConfig;
        public CartesianMapper<TxtDataModel> TxtDataConfig => _txtDataConfig;

        private readonly ICommand _loadTxtDataCommand;
        public ICommand LoadTxtDataCommand => _loadTxtDataCommand;

        public ChartViewModel(ChartParas para)
        {
            try
            {
                // 设置时间格式化器
                DateTimeFormatter = value => new DateTime((long)value).ToString("HH:mm");
                
                // 初始化图表参数
                XTimeSpan = para.XTimeSpan;
                XAxisStep = TimeSpan.FromSeconds(para.XStep).Ticks;
                YAxisStep = para.YStep;
                YAxisMax = para.YMax;
                YAxisMin = para.YMin;

                // 配置TXT数据映射
                _txtDataConfig = new CartesianMapper<TxtDataModel>()
                    .X((model, index) => model.Time)
                    .Y((model, index) => model.Value);

                // 注册配置
                LiveCharts.Charting.For<TxtDataModel>(_txtDataConfig);

                // 初始化命令
                _loadTxtDataCommand = new DelegateCommand(LoadTxtData);

                // 初始化实时数据集合
                Chart1Values = new ChartValues<MeasureData>();
            }
            catch (Exception ex)
            {
                FileTool.SaveFailLog(ex.Message);
            }
            SetAxisLimits(DateTime.Now);
        }
        #endregion

        #region 实时数据显示相关属性
        private ChartValues<MeasureData> _Chart1Values;
        public ChartValues<MeasureData> Chart1Values
        {
            get => _Chart1Values;
            set
            {
                _Chart1Values = value;
                RaisePropertyChanged();
            }
        }

        public Func<double, string> DateTimeFormatter { get; set; }

        private double _axisMax1;
        private double _axisMin1;
        public double AxisMax1
        {
            get => _axisMax1;
            set
            {
                _axisMax1 = value;
                RaisePropertyChanged("AxisMax1");
            }
        }
        public double AxisMin1
        {
            get => _axisMin1;
            set
            {
                _axisMin1 = value;
                RaisePropertyChanged("AxisMin1");
            }
        }

        private double _XAxisStep = TimeSpan.FromMinutes(1).Ticks;
        public double XAxisStep
        {
            get => _XAxisStep;
            set
            {
                _XAxisStep = value;
                RaisePropertyChanged();
            }
        }

        private double _YAxisStep = 5;
        public double YAxisStep
        {
            get => _YAxisStep;
            set
            {
                _YAxisStep = value;
                RaisePropertyChanged();
            }
        }

        private double _YAxisMin = 0;
        public double YAxisMin
        {
            get => _YAxisMin;
            set
            {
                _YAxisMin = value;
                RaisePropertyChanged();
            }
        }

        private double _YAxisMax = 100;
        public double YAxisMax
        {
            get => _YAxisMax;
            set
            {
                _YAxisMax = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region TXT文件数据显示相关属性
        private ObservableCollection<TxtDataModel> _chartData;
        public ObservableCollection<TxtDataModel> ChartData
        {
            get => _chartData;
            set
            {
                _chartData = value;
                RaisePropertyChanged("ChartData");
                
                // 更新坐标轴范围
                if (_chartData != null && _chartData.Count > 0)
                {
                    TxtDataMinX = _chartData.Min(x => x.Time);
                    TxtDataMaxX = _chartData.Max(x => x.Time);
                    TxtDataMinY = _chartData.Min(x => x.Value);
                    TxtDataMaxY = _chartData.Max(x => x.Value);
                }
            }
        }

        private double _txtDataMinX;
        public double TxtDataMinX
        {
            get => _txtDataMinX;
            set
            {
                _txtDataMinX = value;
                RaisePropertyChanged("TxtDataMinX");
            }
        }

        private double _txtDataMaxX;
        public double TxtDataMaxX
        {
            get => _txtDataMaxX;
            set
            {
                _txtDataMaxX = value;
                RaisePropertyChanged("TxtDataMaxX");
            }
        }

        private double _txtDataMinY;
        public double TxtDataMinY
        {
            get => _txtDataMinY;
            set
            {
                _txtDataMinY = value;
                RaisePropertyChanged("TxtDataMinY");
            }
        }

        private double _txtDataMaxY;
        public double TxtDataMaxY
        {
            get => _txtDataMaxY;
            set
            {
                _txtDataMaxY = value;
                RaisePropertyChanged("TxtDataMaxY");
            }
        }

        // 添加格式化器
        public Func<double, string> TxtDataFormatter { get; set; } = value => value.ToString("F2");

        private void LoadTxtData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadTxtData 命令被触发");
                
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    Title = "选择数据文件"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    System.Diagnostics.Debug.WriteLine($"选择的文件: {openFileDialog.FileName}");
                    var data = TxtFileReader.ReadTxtFile(openFileDialog.FileName);
                    if (data != null && data.Count > 0)
                    {
                        // 创建新的 ChartValues 集合
                        var chartValues = new ChartValues<TxtDataModel>(data);
                        ChartData = new ObservableCollection<TxtDataModel>(data);
                        
                        System.Diagnostics.Debug.WriteLine($"加载了 {data.Count} 条数据");
                        System.Diagnostics.Debug.WriteLine($"X范围: {TxtDataMinX} - {TxtDataMaxX}");
                        System.Diagnostics.Debug.WriteLine($"Y范围: {TxtDataMinY} - {TxtDataMaxY}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"加载文件时出错: {ex.Message}", "错误", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        #endregion

        #region 图表控制方法
        public void SetAxisStep(int step)
        {
            XAxisStep = TimeSpan.FromMinutes(step).Ticks;
        }

        private double _AxisUnit = TimeSpan.FromMinutes(1).Ticks;
        public double AxisUnit
        {
            get => _AxisUnit;
            set
            {
                _AxisUnit = value;
                RaisePropertyChanged("AxisUnit");
            }
        }

        public void SetAxisUnit(int step)
        {
            AxisUnit = TimeSpan.FromMinutes(step).Ticks;
        }

        public int XTimeSpan { get; set; } = 300;

        private void SetAxisLimits(DateTime now)
        {
            int start = 20;
            int stop = XTimeSpan;
            AxisMax1 = now.Ticks + TimeSpan.FromSeconds(start).Ticks;
            AxisMin1 = now.Ticks - TimeSpan.FromSeconds(stop).Ticks;
        }
        #endregion

        #region 实时数据处理方法
        private void AddNewData(MeasureData data)
        {
            if (Chart1Values.Count > 100) Chart1Values.RemoveAt(0);
            Chart1Values.Add(data);
            SetAxisLimits(DateTime.Now);
        }

        private double _CurValue;
        public double CurValue
        {
            get => _CurValue;
            set
            {
                _CurValue = value;
                RaisePropertyChanged();
            }
        }

        public void AddData(double value)
        {
            try
            {
                CurValue = value;
                MeasureData data = new MeasureData { DateTime = DateTime.Now, Value = value };
                App.Current.Dispatcher.BeginInvoke(new MethodInvoker(() =>
                {
                    AddNewData(data);
                }));
            }
            catch (Exception ex)
            {
                FileTool.SaveFailLog($"添加数据时出错: {ex.Message}");
            }
        }
        #endregion
    }
} 