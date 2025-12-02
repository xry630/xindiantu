using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Base
{
    // 定义NotificationObject类，继承自INotifyPropertyChanged接口
    public class NotificationObject : INotifyPropertyChanged
    {
        // 声明PropertyChanged事件，这是INotifyPropertyChanged接口的一部分
        public event PropertyChangedEventHandler PropertyChanged;

        // 定义一个RaisePropertyChanged方法，用于通知属性值发生了变化
        public void RaisePropertyChanged([CallerMemberName] string property = "")
        {
            // 检查是否有订阅PropertyChanged事件
            if (PropertyChanged != null)
            {
                // 如果有订阅者，触发PropertyChanged事件，通知属性变化
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
