using ECGViewer.Wpf.Models;

namespace ECGViewer.Wpf.Services
{
    /// <summary>
    /// Interface for data service operations.
    /// </summary>
    public interface IDataService
    {
        Task<List<ECGData>> LoadDataAsync(string filePath);
        Task<bool> SaveDataAsync(string filePath, List<ECGData> data);
    }
}
