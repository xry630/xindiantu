using System;

namespace ECGViewer.Wpf.Timing
{
    /// <summary>
    /// Factory interface for creating dispatcher timers
    /// </summary>
    public interface IDispatcherTimerFactory
    {
        /// <summary>
        /// Creates a new dispatcher timer
        /// </summary>
        /// <returns>A new instance of IDispatcherTimer</returns>
        IDispatcherTimer CreateTimer();
    }
}