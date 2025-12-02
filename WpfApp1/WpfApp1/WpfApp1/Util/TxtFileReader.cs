using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using WpfApp1.Models;

namespace WpfApp1.Util
{
    public class TxtFileReader
    {
        public static List<TxtDataModel> ReadTxtFile(string filePath)
        {
            var dataList = new List<TxtDataModel>();
            
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    // 忽略注释行和空行
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("//"))
                        continue;

                    // 分割并清理数据
                    string[] values = line.Split(',')
                                        .Select(s => s.Trim())
                                        .ToArray();

                    if (values.Length >= 2 && 
                        double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double time) && 
                        double.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        var dataPoint = new TxtDataModel
                        {
                            Time = time,
                            Value = value
                        };
                        dataList.Add(dataPoint);
                    }
                }

                if (dataList.Count == 0)
                {
                    throw new Exception("未能从文件中读取到有效数据");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"读取文件出错: {ex.Message}", "错误", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }
            
            return dataList;
        }
    }
} 