namespace ECGViewer.Wpf.Models
{
    /// <summary>
    /// Represents ECG data points.
    /// </summary>
    public class ECGData
    {
        public int Index { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
