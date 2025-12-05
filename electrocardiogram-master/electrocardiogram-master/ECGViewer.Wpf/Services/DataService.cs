using ECGViewer.Wpf.Models;

namespace ECGViewer.Wpf.Services
{
    /// <summary>
    /// Implementation of IDataService for loading and saving ECG data.
    /// </summary>
    public class DataService : IDataService
    {
        public async Task<List<ECGData>> LoadDataAsync(string filePath)
        {
            var data = new List<ECGData>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Data file not found: {filePath}");
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                int index = 0;
                foreach (var line in lines)
                {
                    if (double.TryParse(line.Trim(), out var value))
                    {
                        data.Add(new ECGData
                        {
                            Index = index++,
                            Value = value,
                            Timestamp = DateTime.Now.AddMilliseconds(index)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading data from {filePath}", ex);
            }

            return data;
        }

        public async Task<bool> SaveDataAsync(string filePath, List<ECGData> data)
        {
            try
            {
                var lines = data.Select(d => d.Value.ToString()).ToArray();
                await File.WriteAllLinesAsync(filePath, lines);
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving data to {filePath}", ex);
            }
        }
    }
}
