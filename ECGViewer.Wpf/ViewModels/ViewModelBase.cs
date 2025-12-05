using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ECGViewer.Wpf.ViewModels
{
    /// <summary>
    /// Base class for all view models implementing INotifyPropertyChanged and INotifyDataErrorInfo
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo, IDisposable
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly SynchronizationContext _synchronizationContext;

        protected ViewModelBase()
        {
            _synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the property value and raises PropertyChanged event if the value has changed
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">New value</param>
        /// <param name="propertyName">Name of the property (automatically provided)</param>
        /// <returns>True if the value was changed, false otherwise</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the property value, raises PropertyChanged event, and validates the property
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">New value</param>
        /// <param name="propertyName">Name of the property (automatically provided)</param>
        /// <returns>True if the value was changed, false otherwise</returns>
        protected bool SetPropertyAndValidate<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            bool changed = SetProperty(ref field, value, propertyName);
            if (changed)
            {
                ValidateProperty(propertyName, value);
            }
            return changed;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (SynchronizationContext.Current != _synchronizationContext)
            {
                _synchronizationContext.Post(_ => 
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }, null);
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises PropertyChanged event for multiple properties
        /// </summary>
        /// <param name="propertyNames">Names of properties that changed</param>
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        #endregion

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errors.Count > 0;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.Values;
            }

            if (_errors.TryGetValue(propertyName, out List<string> errors))
            {
                return errors;
            }

            return null;
        }

        /// <summary>
        /// Adds an error for a specific property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="error">Error message</param>
        protected void AddError(string propertyName, string error)
        {
            if (!_errors.TryGetValue(propertyName, out List<string> errors))
            {
                errors = new List<string>();
                _errors[propertyName] = errors;
            }

            if (!errors.Contains(error))
            {
                errors.Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Removes all errors for a specific property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Removes all errors for all properties
        /// </summary>
        protected void ClearAllErrors()
        {
            var propertyNames = new List<string>(_errors.Keys);
            _errors.Clear();
            
            foreach (var propertyName in propertyNames)
            {
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Validates a property (override in derived classes to implement validation logic)
        /// </summary>
        /// <param name="propertyName">Name of the property to validate</param>
        /// <param name="value">Value to validate</param>
        protected virtual void ValidateProperty(string propertyName, object value)
        {
            // Override in derived classes to implement validation logic
        }

        /// <summary>
        /// Raises the ErrorsChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property whose errors changed</param>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            if (SynchronizationContext.Current != _synchronizationContext)
            {
                _synchronizationContext.Post(_ => 
                {
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }, null);
            }
            else
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Executes an action on the UI thread
        /// </summary>
        /// <param name="action">Action to execute</param>
        protected void InvokeOnMainThread(Action action)
        {
            if (SynchronizationContext.Current != _synchronizationContext)
            {
                _synchronizationContext.Post(_ => action(), null);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Executes a function on the UI thread and returns the result
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Function to execute</param>
        /// <returns>Result of the function</returns>
        protected T InvokeOnMainThread<T>(Func<T> func)
        {
            if (SynchronizationContext.Current != _synchronizationContext)
            {
                T result = default(T);
                _synchronizationContext.Post(_ => result = func(), null);
                return result;
            }
            else
            {
                return func();
            }
        }

        #endregion

        #region IDisposable

        private bool _disposed = false;

        /// <summary>
        /// Releases all resources used by the ViewModelBase
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _errors.Clear();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}