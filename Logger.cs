using System;
using System.IO;
using System.Threading.Tasks;

class Logger
{
    private static readonly object lockObj = new object();
    private static Logger instance = null;
    private readonly string filePath;

    private Logger(string fileName)
    {
        string directoryPath = Path.GetTempPath();
        filePath = Path.Combine(directoryPath, fileName);

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
    }

    public static Logger GetInstance()
    {
        lock (lockObj)
        {
            if (instance == null)
            {
                string fileName = $"swiftmac-debug-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log";
                instance = new Logger(fileName);
            }
            return instance;
        }
    }

    public async Task Log(string message)
    {
#if DEBUG
        await Task.Run(() =>
        {
            lock (lockObj)
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
            }
        });
#endif
    }
}
