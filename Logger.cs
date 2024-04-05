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
        if (instance == null)
        {
            lock (lockObj)
            {
                if (instance == null)
                {
                    string fileName = $"swiftmac-debug-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log";
                    instance = new Logger(fileName);
                }
            }
        }
        return instance;
    }

    public async Task Log(string message)
    {
#if DEBUG
        // Utilize the asynchronous API for file operations
        await WriteLogAsync(message);
#endif
    }

    private async Task WriteLogAsync(string message)
    {
        // Locking over a smaller scope
        // We use a semaphore slim or another async-compatible locking mechanism if needed for finer control
        lock (lockObj)
        {
            // Dummy lock to illustrate point, ideally we'd use SemaphoreSlim for async lock
        }
        try
        {
            // Async file write operation
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}
