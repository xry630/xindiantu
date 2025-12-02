using System;  // 引入基本的系统功能
using System.Collections.Generic;  // 引入泛型集合类
using System.IO;  // 引入文件操作功能
using System.Linq;  // 引入LINQ（语言集成查询）功能
using System.Globalization;  // 引入文化信息功能，处理地区相关的格式化
using WpfApp1.Models;  // 引入应用程序中的数据模型类

namespace WpfApp1.Util  // 定义命名空间，包含工具类
{
    public class TxtFileReader  // 定义一个文本文件读取类
    {
        // 静态方法，用于读取文本文件并返回一个 MeasureData 类型的列表
        public static List<MeasureData> ReadTxtFile(string filePath)
        {
            var dataList = new List<MeasureData>();  // 创建一个空的列表，用于存储解析后的数据点

            try
            {

                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("//"))
                        continue;

                    // 修改分割和解析逻辑
                    string[] values = line.Split(',')
                                        .Select(s => s.Trim())
                                        .Where(s => !string.IsNullOrEmpty(s))  // 过滤空字符串
                                        .ToArray();

                    if (values.Length > 0)  // 修改为至少一个数据项
                    {
                        foreach (var valueStr in values)  // 遍历所有值
                        {
                            if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))  // 尝试将值转换为 double 类型
                            {
                                // 创建一个 MeasureData 对象
                                var dataPoint = new MeasureData
                                {
                                    Value = value  // 设置数值属性
                                };
                                dataList.Add(dataPoint);  // 将解析的数据点添加到列表中
                            }
                        }
                    }

                    // 如果列表中没有有效数据，抛出异常
                    if (dataList.Count == 0)
                    {
                        throw new Exception("未能从文件中读取到有效数据");  // 如果没有有效数据，抛出异常
                    }
                }
            }
            catch (Exception ex)  // 捕获异常
            {
                // 弹出消息框显示错误信息
                System.Windows.MessageBox.Show($"读取文件出错: {ex.Message}", "错误",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);  // 显示错误信息，通知用户
                return null;  // 返回 null 表示发生错误
            }

            return dataList;  // 返回有效数据列表
        }
    }
}



//│   │   ├── Util /                            # 工具类
//│   │   │   ├── TxtFileReader              # 文本文件读取类
//│   │   │   │   ├── 静态方法 ReadTxtFile   # 读取文本文件并返回数据列表的方法
//│   │   │   │   │   ├── dataList           # 存储读取到的有效数据的列表
//│   │   │   │   │   ├── try-catch 结构     # 错误处理结构，用于捕获文件读取和数据解析错误
//│   │   │   │   │   │   ├── File.ReadAllLines(filePath)  # 读取文件所有行
//│   │   │   │   │   │   ├── foreach 遍历行  # 遍历每一行文本
//│   │   │   │   │   │   │   ├── 忽略空行和注释行判断   # 忽略空行和以“//”开头的注释行
//│   │   │   │   │   │   │   ├── Split 和 Trim 数据处理   # 分割每一行数据并去除空格
//│   │   │   │   │   │   │   ├── double.TryParse 解析数据   # 尝试将数据转换为 double 类型
//│   │   │   │   │   │   │   ├── 创建 TxtDataModel 实例   # 创建 TxtDataModel 数据点对象
//│   │   │   │   │   │   │   ├── 添加数据点到列表         # 将有效数据点添加到 dataList 列表
//│   │   │   │   │   │   ├── 判断无有效数据并抛出异常     # 如果没有有效数据，抛出异常
//│   │   │   │   │   │   ├── 弹出消息框显示错误信息       # 捕获异常并弹出消息框显示错误信息
//│   │   │   │   │   ├── 返回解析后的有效数据列表         # 返回包含有效数据点的列表









