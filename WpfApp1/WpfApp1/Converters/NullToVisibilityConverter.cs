using System;  // 引入系统命名空间，包含基本功能和数据类型
using System.Windows;  // 引入WPF的核心命名空间，包含UI元素和相关功能
using System.Windows.Data;  // 引入数据绑定相关的命名空间，包含IValueConverter接口

namespace WpfApp1.Converters  // 定义命名空间WpfApp1.Converters，存放转换器类
{
    // 定义NullToVisibilityConverter类，继承自IValueConverter接口
    public class NullToVisibilityConverter : IValueConverter
    {
        // Convert方法将源值转换为目标值
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // 如果传入的值为null，返回Collapsed（不可见），否则返回Visible（可见）
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        // ConvertBack方法用于将目标值转换回源值，通常不需要实现（因为在这种情况下不需要双向绑定）
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // 如果需要反向转换（比如双向绑定），会抛出异常，因为本转换器不支持反向转换
            throw new NotImplementedException();
        }
    }
}
