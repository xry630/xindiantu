using System.IO;          // 文件输入输出操作
using System.Windows;     // WPF基础类库
using System.Windows.Threading;  // 定时器相关
using Microsoft.Win32;   // 用于打开文件对话框
using System.Threading.Tasks; // 用于异步编程
using System.Collections.Generic; // 用于集合

namespace DrawCurveDemo
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer pushFhrDataTimer;
        private List<int> fhrDatas = new(); // 使用 List 存储心率数据
        private int timeIndex = 1;
        private int dataIndex = 0;
        private bool isDataLoaded = false; // 数据加载状态标志

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            pushFhrDataTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            pushFhrDataTimer.Tick += PushFhrDataTimer_Tick;
        }

        /// <summary>
        /// 加载按钮点击事件处理
        /// </summary>
        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDataLoaded)
            {
                MessageBox.Show("数据已加载，无需重复加载。", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            await LoadFetalHeartRateDataAsync();
        }

        /// <summary>
        /// 从外部文件异步加载胎儿心率数据
        /// </summary>
        private async Task LoadFetalHeartRateDataAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "选择胎儿心率数据文件"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    using StreamReader reader = new StreamReader(filePath);
                    string line;

                    // 清空之前的数据
                    fhrDatas.Clear();

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var splitValues = line.Split(',');
                        foreach (var value in splitValues)
                        {
                            if (int.TryParse(value, out int heartRate))
                            {
                                fhrDatas.Add(heartRate); // 添加到数据集
                            }
                        }
                    }

                    // 检查 fhrDatas 列表是否为空
                    if (fhrDatas.Count > 0)
                    {
                        isDataLoaded = true; // 设置数据加载状态为已加载
                        pushFhrDataTimer.Start(); // 启动定时器
                        MessageBox.Show("数据加载成功！", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("加载的数据为空，请检查文件内容。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"读取文件时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 定时器触发事件处理
        /// </summary>
        private void PushFhrDataTimer_Tick(object sender, EventArgs e)
        {
            if (!isDataLoaded)
            {
                MessageBox.Show("没有加载任何胎儿心率数据，请先加载数据。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 创建4个数据单元的数组（每秒推送4个数据点）
            var fhrs = new FhrModel[4];

            for (int i = 0; i < fhrs.Length; i++)
            {
                if (dataIndex < fhrDatas.Count)
                {
                    fhrs[i] = new FhrModel
                    {
                        Index = timeIndex,
                        Fhr1 = fhrDatas[dataIndex],
                        Fhr2 = fhrDatas[dataIndex],
                        Fhr3 = fhrDatas[dataIndex],
                    };

                    timeIndex++;
                    dataIndex++;
                }
                //else
                //{
                //    dataIndex = 0; // 重置索引
                //    i--; // 重新填充当前数据单元
                //}
            }

            // 调用 FhrCurveChart 的方法添加数据
            chart.AddFhrData(fhrs);
        }
    }
}