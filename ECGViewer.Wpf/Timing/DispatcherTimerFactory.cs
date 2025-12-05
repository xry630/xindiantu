using System.Windows.Threading;

namespace ECGViewer.Wpf.Timing
{
    /// <summary>
    /// WPF implementation of IDispatcherTimerFactory
    /// </summary>
    public class DispatcherTimerFactory : IDispatcherTimerFactory
    {
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the DispatcherTimerFactory class
        /// </summary>
        public DispatcherTimerFactory()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Initializes a new instance of the DispatcherTimerFactory class with a specific dispatcher
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use for creating timers</param>
        public DispatcherTimerFactory(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Creates a new dispatcher timer
        /// </summary>
        /// <returns>A new instance of IDispatcherTimer</returns>
        public IDispatcherTimer CreateTimer()
        {
            return new DispatcherTimerAdapter(_dispatcher);
        }
    }
}