using System;
using System.Windows.Input;
using WpfApp1.Base;

namespace WpfApp1.Models
{
    //public class FastCmdModel
    //{
    //    public string Name { get; set; }
    //    public string Command { get; set; }

    //    private ICommand _executeCommand;
    //    public ICommand ExecuteCommand
    //    {
    //        get
    //        {
    //            if (_executeCommand == null)
    //            {
    //                _executeCommand = new DelegateCommand<string>(ExecuteCommandAction);
    //            }
    //            return _executeCommand;
    //        }
    //    }

    //    private void ExecuteCommandAction(string parameter)
    //    {
    //        try
    //        {
    //            // 使用Command属性和parameter参数执行命令
    //            string commandToExecute = parameter ?? Command;
    //            // 在这里添加执行命令的代码
    //        }
    //        catch (Exception ex)
    //        {
    //            System.Windows.MessageBox.Show($"执行命令时出错: {ex.Message}");
    //        }
    //    }
    //}

    public class FastCmdModel : NotificationObject
    {
        // 如果你已经定义了 SendFastCmd 方法，就不需要再次定义为委托
        // 如果需要发送命令的方法，使用如下方法定义
        public bool SendFastCmd(string cmd)
        {
            // 执行发送命令的逻辑（这里简单打印命令，可以根据需求更改为实际的发送逻辑）
            Console.WriteLine($"发送命令: {cmd}");
            return true;  // 返回命令执行是否成功，这里返回 true 表示成功
        }

        // 委托类型，用于发送命令的委托
        public delegate bool SendFastCmdDelegate(string cmd);

        public FastCmdModel()
        {
            // 初始化 SendCmd 委托，可以根据实际需求提供实际的命令处理逻辑
            SendCmd = cmd => {
                // 执行命令的具体逻辑，实际使用时可以替换为你的发送命令方法
                Console.WriteLine($"发送命令: {cmd}");
                return true; // 返回命令执行是否成功
            };

            // 正确初始化 DelegateCommand，并传递 SendFastCmd 方法作为执行方法
            // 这里传递的是 SendFastCmd 方法，可以根据需求调整
            SendFastCmdCmd = new DelegateCommand(SendFastCmd);
        }

        // SendCmd 委托实例，可以用于其他地方调用发送命令的逻辑
        [Newtonsoft.Json.JsonIgnore()]
        public SendFastCmdDelegate SendCmd;

        private int _CmdNum = 0;
        /// <summary>
        /// 命令编号
        /// </summary>
        public int CmdNum
        {
            get { return _CmdNum; }
            set
            {
                _CmdNum = value;
                RaisePropertyChanged("CmdNum");  // 通知 UI 更新命令编号
                CmdNumStr = SetCmdNumStr(value);  // 更新命令编号字符串
            }
        }

        private string SetCmdNumStr(int cmdnum)
        {
            string str = cmdnum.ToString();
            if (str.Length == 1)
            {
                return "0" + str;  // 如果命令编号是单一数字，补充前导零
            }
            else
            {
                return str;
            }
        }

        private string _CmdNumStr = "";
        /// <summary>
        /// 命令编号字符串
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        public string CmdNumStr
        {
            get
            {
                _CmdNumStr = SetCmdNumStr(CmdNum);  // 计算并返回命令编号的字符串
                return _CmdNumStr;
            }
            set
            {
                _CmdNumStr = value;
                RaisePropertyChanged("CmdNumStr");  // 通知 UI 更新命令编号字符串
            }
        }

        private int _RowID = 0;
        /// <summary>
        /// 命令所在行ID
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        public int RowID
        {
            get { return _RowID; }
            set
            {
                _RowID = value;
                RaisePropertyChanged("RowID");  // 通知 UI 更新行号
            }
        }

        private int _DelayTime = 50;

        public int DelayTime
        {
            get { return _DelayTime; }
            set
            {
                _DelayTime = value;
                RaisePropertyChanged("DelayTime");  // 通知 UI 更新延迟时间
            }
        }

        private string _CmdString = "";

        public string CmdString
        {
            get { return _CmdString; }
            set
            {
                _CmdString = value;
                RaisePropertyChanged("CmdString");  // 通知 UI 更新命令字符串
            }
        }

        private string _Remark = "";
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set
            {
                _Remark = value;
                RaisePropertyChanged("Remark");  // 通知 UI 更新备注
            }
        }

        private bool _IsSelected = false;
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");  // 通知 UI 更新选中状态
            }
        }

        // DelegateCommand 用于绑定命令，执行命令
        [Newtonsoft.Json.JsonIgnore()]
        public DelegateCommand SendFastCmdCmd { get; private set; }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="para">传递的命令参数</param>
        public void SendFastCmd(object para)
        {
            if (SendCmd != null)
            {
                try
                {
                    // 执行发送命令
                    SendCmd(CmdString);  // 使用委托发送命令
                }
                catch
                {
                    // 错误处理（例如日志记录），当前没有实现
                }
            }
        }
    }

}