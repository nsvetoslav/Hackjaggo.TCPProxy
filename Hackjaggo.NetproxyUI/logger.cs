namespace Hackjaggo.Proxy
{
    public enum LogType
    {
        INFO,
        WARNING,
        ERROR
    }

    public static class Logger
    {
        private static string logFilePath { get; set; } = string.Empty;
        private static string currentLogDate { get; set; } = string.Empty;

        private static void UpdateLogFilePath()
        {
            string logDate = DateTime.Now.ToString("yyyy-MM-dd");
            if (logDate != currentLogDate)
            {
                currentLogDate = logDate;
                logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log_{currentLogDate}.log");
            }
        }

        public static void Log(LogType logType, string message)
        {
            UpdateLogFilePath();
            string logMessage = $"{DateTime.Now:HH:mm:ss} [{logType}]: {message}";

            try
            {
                // Check if the file exists, create it if it doesn't
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath).Close();
                }

                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }

        public static void LogInfo(string message)
        {
            Log(LogType.INFO, message);
        }

        public static void LogWarning(string message)
        {
            Log(LogType.WARNING, message);
        }

        public static void LogError(string message)
        {
            Log(LogType.ERROR, message);
        }
    }
}
