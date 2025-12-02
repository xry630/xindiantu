using System;
using System.Windows.Input;
using WpfApp1.Base;

namespace WpfApp1.Models
{
    public class FastCmdModel
    {
        public string Name { get; set; }
        public string Command { get; set; }

        private ICommand _executeCommand;
        public ICommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new DelegateCommand<string>(ExecuteCommandAction);
                }
                return _executeCommand;
            }
        }

        private void ExecuteCommandAction(string parameter)
        {
            try
            {
                // 使用Command属性和parameter参数执行命令
                string commandToExecute = parameter ?? Command;
                // 在这里添加执行命令的代码
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"执行命令时出错: {ex.Message}");
            }
        }
    }
} 