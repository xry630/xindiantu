using System;
using System.IO;
using System.Collections.Generic;

namespace MsPaint
{
    /// <summary>
    /// 心电图数据加载服务类
    /// 负责从TXT文件中读取心电图数据并进行预处理
    /// </summary>
    public class DataLoader
    {
        /// <summary>
        /// 从TXT文件加载心电图数据
        /// </summary>
        /// <param name="filename">TXT文件路径</param>
        /// <returns>心电图数据数组</returns>
        public static double[,] LoadDataFromTxt(string filename)
        {
            try
            {
                // 读取TXT文件中的所有行
                string[] lines = File.ReadAllLines(filename);
                List<double> dataList = new List<double>();

                // 解析每一行的数值
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine))
                    {
                        if (double.TryParse(trimmedLine, out double value))
                        {
                            dataList.Add(value);
                        }
                    }
                }

                // 转换为二维数组格式（与MAT文件格式保持一致）
                double[,] result = new double[1, dataList.Count];
                for (int i = 0; i < dataList.Count; i++)
                {
                    result[0, i] = dataList[i];
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"加载TXT文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 计算心电图特征点（R、Q、T波位置）
        /// 这是一个简化的算法，实际应用中可能需要更复杂的信号处理算法
        /// </summary>
        /// <param name="data">心电图数据</param>
        /// <returns>包含R、Q、T波位置的数据</returns>
        public static ECGFeatures CalculateFeatures(double[,] data)
        {
            int dataLength = data.GetLength(1);
            
            // 简化的R波检测算法（峰值检测）
            List<int> rPeaks = new List<int>();
            List<int> qPeaks = new List<int>();
            List<int> tPeaks = new List<int>();

            // 使用滑动窗口检测R波峰值
            int windowSize = 50; // 窗口大小
            double threshold = 0.6; // 阈值

            for (int i = windowSize; i < dataLength - windowSize; i++)
            {
                double currentValue = data[0, i];
                bool isPeak = true;

                // 检查当前点是否为局部最大值
                for (int j = i - windowSize; j <= i + windowSize; j++)
                {
                    if (j != i && data[0, j] >= currentValue)
                    {
                        isPeak = false;
                        break;
                    }
                }

                // 如果检测到峰值且超过阈值，认为是R波
                if (isPeak && currentValue > threshold)
                {
                    rPeaks.Add(i);
                    
                    // 简化的Q波检测（R波前的局部最小值）
                    int qIndex = FindQPeak(data, i, windowSize);
                    if (qIndex > 0)
                    {
                        qPeaks.Add(qIndex);
                    }

                    // 简化的T波检测（R波后的局部最大值）
                    int tIndex = FindTPeak(data, i, windowSize);
                    if (tIndex > 0)
                    {
                        tPeaks.Add(tIndex);
                    }
                }
            }

            // 转换为数组格式
            double[,] rbegin = new double[1, Math.Min(qPeaks.Count, 60)];
            double[,] pend = new double[1, Math.Min(tPeaks.Count, 60)];
            double[,] rpk = new double[1, Math.Min(rPeaks.Count, 60)];

            for (int i = 0; i < Math.Min(qPeaks.Count, 60); i++)
            {
                rbegin[0, i] = qPeaks[i] * 5; // 转换为与原始代码相同的格式
            }

            for (int i = 0; i < Math.Min(tPeaks.Count, 60); i++)
            {
                pend[0, i] = tPeaks[i] * 5;
            }

            for (int i = 0; i < Math.Min(rPeaks.Count, 60); i++)
            {
                rpk[0, i] = rPeaks[i] * 5;
            }

            // 计算平均QT间期
            double qtMean = 0;
            int validCount = 0;
            for (int i = 0; i < Math.Min(qPeaks.Count, tPeaks.Count); i++)
            {
                if (tPeaks[i] > qPeaks[i])
                {
                    qtMean += (tPeaks[i] - qPeaks[i]) * 0.001; // 转换为秒
                    validCount++;
                }
            }
            qtMean = validCount > 0 ? qtMean / validCount : 0.4; // 默认值

            return new ECGFeatures
            {
                Rbegin = rbegin,
                Pend = pend,
                Rpk = rpk,
                QtMean = qtMean
            };
        }

        /// <summary>
        /// 查找Q波位置（R波前的局部最小值）
        /// </summary>
        private static int FindQPeak(double[,] data, int rIndex, int windowSize)
        {
            int startIndex = Math.Max(0, rIndex - windowSize);
            int minIndex = startIndex;
            double minValue = data[0, startIndex];

            for (int i = startIndex; i < rIndex; i++)
            {
                if (data[0, i] < minValue)
                {
                    minValue = data[0, i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        /// <summary>
        /// 查找T波位置（R波后的局部最大值）
        /// </summary>
        private static int FindTPeak(double[,] data, int rIndex, int windowSize)
        {
            int endIndex = Math.Min(data.GetLength(1) - 1, rIndex + windowSize);
            int maxIndex = rIndex;
            double maxValue = data[0, rIndex];

            for (int i = rIndex; i <= endIndex; i++)
            {
                if (data[0, i] > maxValue)
                {
                    maxValue = data[0, i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }
    }

    /// <summary>
    /// 心电图特征数据结构
    /// </summary>
    public class ECGFeatures
    {
        public double[,] Rbegin { get; set; } // Q波位置
        public double[,] Pend { get; set; }   // T波位置
        public double[,] Rpk { get; set; }    // R波位置
        public double QtMean { get; set; }    // 平均QT间期
    }
} 