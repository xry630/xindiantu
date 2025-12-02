using System;
using System.Windows.Input;
using WpfApp1.Base;

namespace WpfApp1.Base
{
    // 定义一个委托命令类，实现ICommand接口
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeMethod; // 执行方法
        private readonly Func<object, bool> _canExecuteMethod; // 判断是否可以执行的方法

        // 构造函数，接受无参数的执行方法
        public DelegateCommand(Action executeMethod)
            : this((o) => executeMethod(), null)
        {
        }

        // 构造函数，接受带参数的执行方法
        public DelegateCommand(Action<object> executeMethod)
            : this(executeMethod, null)
        {
        }

        // 构造函数，接受无参数的执行方法和判断是否可以执行的方法
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this((o) => executeMethod(), (o) => canExecuteMethod())
        {
        }

        // 构造函数，接受带参数的执行方法和判断是否可以执行的方法
        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod"); // 如果执行方法为空，抛出异常
            }
            _executeMethod = executeMethod; // 初始化执行方法
            _canExecuteMethod = canExecuteMethod; // 初始化判断是否可以执行的方法
        }

        #region ICommand 成员

        // CanExecuteChanged 事件，当 CanExecute 的结果发生变化时触发
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value; // 注册 CanExecuteChanged 事件
                }
            }
            remove
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value; // 注销 CanExecuteChanged 事件
                }
            }
        }

        // 判断命令是否可以执行
        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod == null || _canExecuteMethod(parameter); // 如果没有CanExecute方法，返回true
        }

        // 执行命令
        public void Execute(object parameter)
        {
            if (CanExecute(parameter)) // 如果可以执行
            {
                _executeMethod(parameter); // 调用执行方法
            }
        }

        #endregion

        // 手动触发 CanExecuteChanged 事件
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested(); // 触发 CanExecute 的重新查询
        }
    }

    // 泛型版本的 DelegateCommand，实现 ICommand 接口
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _executeMethod; // 执行方法，带泛型参数
        private readonly Func<T, bool> _canExecuteMethod; // 判断是否可以执行的方法，带泛型参数

        // 构造函数，接受一个执行方法，默认没有CanExecute方法
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        // 构造函数，接受一个执行方法和判断是否可以执行的方法
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod"); // 如果执行方法为空，抛出异常
            }
            _executeMethod = executeMethod; // 初始化执行方法
            _canExecuteMethod = canExecuteMethod; // 初始化判断是否可以执行的方法
        }

        // CanExecuteChanged 事件
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value; // 注册事件
                }
            }
            remove
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value; // 注销事件
                }
            }
        }

        // 判断命令是否可以执行
        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            {
                return false; // 如果参数为空并且泛型类型是值类型，则不能执行
            }

            return _canExecuteMethod == null || _canExecuteMethod((T)parameter); // 如果没有CanExecute方法，返回true
        }

        // 执行命令
        public void Execute(object parameter)
        {
            if (CanExecute(parameter)) // 如果可以执行
            {
                _executeMethod((T)parameter); // 调用执行方法
            }
        }

        // 手动触发 CanExecuteChanged 事件
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested(); // 触发 CanExecute 的重新查询
        }
    }
}



//│   │   ├── Base /                            # 基础功能类
//│   │   │   ├── DelegateCommand          # 命令模式类
//│   │   │   │   ├── 委托执行方法字段 (_executeMethod)        # 存储执行方法的字段
//│   │   │   │   ├── 可执行判断方法字段 (_canExecuteMethod)  # 存储判断命令是否可以执行的方法字段
//│   │   │   │   ├── 构造函数              # 多重构造函数用于初始化类的不同方式
//│   │   │   │   │   ├── 无参数执行方法构造函数           # 接受无参数执行方法的构造函数
//│   │   │   │   │   ├── 带参数执行方法构造函数           # 接受带参数执行方法的构造函数
//│   │   │   │   │   ├── 无参数执行方法和判断条件构造函数 # 接受无参数执行方法和判断条件的构造函数
//│   │   │   │   │   ├── 带参数执行方法和判断条件构造函数 # 接受带参数执行方法和判断条件的构造函数
//│   │   │   │   ├── ICommand 成员实现     # 实现 ICommand 接口的相关成员
//│   │   │   │   │   ├── CanExecuteChanged 事件          # 处理 CanExecute 改变时的事件
//│   │   │   │   │   ├── CanExecute 方法                # 判断是否可以执行命令的方法
//│   │   │   │   │   ├── Execute 方法                  # 执行命令的方法
//│   │   │   │   ├── 手动触发 CanExecuteChanged 方法    # 触发 CanExecuteChanged 事件的方法
//│   │   │   ├── DelegateCommand<T>      # 泛型版本的命令模式类
//│   │   │   │   ├── 泛型执行方法字段 (_executeMethod)      # 存储泛型执行方法的字段
//│   │   │   │   ├── 泛型可执行判断方法字段 (_canExecuteMethod) # 存储泛型判断命令是否可以执行的方法字段
//│   │   │   │   ├── 构造函数              # 泛型构造函数初始化
//│   │   │   │   │   ├── 仅有执行方法的构造函数       # 只接受执行方法的构造函数
//│   │   │   │   │   ├── 带判断条件的执行方法构造函数 # 接受执行方法和判断条件的构造函数
//│   │   │   │   ├── ICommand 成员实现     # 实现 ICommand 接口的相关成员
//│   │   │   │   │   ├── CanExecuteChanged 事件         # 处理 CanExecute 改变时的事件
//│   │   │   │   │   ├── CanExecute 方法                # 判断是否可以执行命令的方法
//│   │   │   │   │   ├── Execute 方法                  # 执行命令的方法
//│   │   │   │   ├── 手动触发 CanExecuteChanged 方法    # 触发 CanExecuteChanged 事件的方法
