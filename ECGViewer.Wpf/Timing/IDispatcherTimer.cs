using System;

namespace ECGViewer.Wpf.Timing
{
    /// <summary>
    /// Interface for a timer that can be used in view models
    /// </summary>
    public interface IDispatcherTimer : IDisposable
    {
        /// <summary>
        /// Gets or sets the interval between timer ticks
        /// </summary>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets or sets whether the timer is running
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Occurs when the timer interval has elapsed
        /// </summary>
        event EventHandler Tick;

        /// <summary>
        /// Starts the timer
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer
        /// </summary>
        void Stop();
    }
}