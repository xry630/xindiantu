using System;
using System.Windows.Input;

namespace WpfApp1.Base
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeMethod;
        private readonly Func<object, bool> _canExecuteMethod;

        public DelegateCommand(Action executeMethod)
            : this((o) => executeMethod(), null)
        {
        }

        public DelegateCommand(Action<object> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this((o) => executeMethod(), (o) => canExecuteMethod())
        {
        }

        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod == null || _canExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _executeMethod(parameter);
            }
        }

        #endregion

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _executeMethod;
        private readonly Func<T, bool> _canExecuteMethod;

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            {
                return false;
            }

            return _canExecuteMethod == null || _canExecuteMethod((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _executeMethod((T)parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
} 