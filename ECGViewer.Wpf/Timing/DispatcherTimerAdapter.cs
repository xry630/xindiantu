using System;
using System.Windows.Threading;

namespace ECGViewer.Wpf.Timing
{
    /// <summary>
    /// WPF DispatcherTimer implementation of IDispatcherTimer
    /// </summary>
    public class DispatcherTimerAdapter : IDispatcherTimer
    {
        private readonly DispatcherTimer _dispatcherTimer;

        /// <summary>
        /// Initializes a new instance of the DispatcherTimerAdapter class
        /// </summary>
        public DispatcherTimerAdapter()
        {
            _dispatcherTimer = new DispatcherTimer();
        }

        /// <summary>
        /// Initializes a new instance of the DispatcherTimerAdapter class with a specific dispatcher
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use</param>
        public DispatcherTimerAdapter(Dispatcher dispatcher)
        {
            _dispatcherTimer = new DispatcherTimer(dispatcher);
        }

        /// <summary>
        /// Gets or sets the interval between timer ticks
        /// </summary>
        public TimeSpan Interval
        {
            get => _dispatcherTimer.Interval;
            set => _dispatcherTimer.Interval = value;
        }

        /// <summary>
        /// Gets or sets whether the timer is running
        /// </summary>
        public bool IsEnabled
        {
            get => _dispatcherTimer.IsEnabled;
            set
            {
                if (value)
                    _dispatcherTimer.Start();
                else
                    _dispatcherTimer.Stop();
            }
        }

        /// <summary>
        /// Occurs when the timer interval has elapsed
        /// </summary>
        public event EventHandler Tick
        {
            add => _dispatcherTimer.Tick += value;
            remove => _dispatcherTimer.Tick -= value;
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            _dispatcherTimer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            _dispatcherTimer.Stop();
        }

        /// <summary>
        /// Releases all resources used by the DispatcherTimerAdapter
        /// </summary>
        public void Dispose()
        {
            _dispatcherTimer.Stop();
        }
    }
}