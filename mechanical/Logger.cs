using System;
using System.IO;
using System.Threading.Tasks;

class Logger
{
    private string filePath;

    public Logger(string fileName)
    {
        string directoryPath = Path.GetTempPath();
        filePath = Path.Combine(directoryPath, fileName);

        // Create file if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
    }

    public async void Log(string message)
    {
#if DEBUG
        await Task.Run(() =>
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        });
#endif
    }
}
