using System;
using System.Globalization;
using System.IO;

namespace IndustrialBackup.Services
{
    public class BackupLogger
    {
        private const int LogRetentionDays = 30;
        private readonly string _logDirectory;
        private readonly object _syncRoot = new();

        public BackupLogger(string appDataFolder)
        {
            _logDirectory = Path.Combine(appDataFolder, "Logs");

            try
            {
                Directory.CreateDirectory(_logDirectory);
                CleanupOldLogs();
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public void Write(string message)
        {
            lock (_syncRoot)
            {
                try
                {
                    Directory.CreateDirectory(_logDirectory);
                    string filePath = Path.Combine(_logDirectory, $"backup_{DateTime.Now:yyyyMMdd}.log");
                    string formatted = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                    File.AppendAllText(filePath, formatted);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private void CleanupOldLogs()
        {
            lock (_syncRoot)
            {
                if (!Directory.Exists(_logDirectory))
                    return;

                DateTime cutoffDate = DateTime.Now.Date.AddDays(-LogRetentionDays);

                foreach (string filePath in Directory.GetFiles(_logDirectory, "backup_*.log"))
                {
                    try
                    {
                        if (IsOlderThanRetention(filePath, cutoffDate))
                            File.Delete(filePath);
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
        }

        private static bool IsOlderThanRetention(string filePath, DateTime cutoffDate)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            const string prefix = "backup_";

            if (fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && DateTime.TryParseExact(
                    fileName[prefix.Length..],
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime logDate))
            {
                return logDate.Date < cutoffDate;
            }

            return File.GetLastWriteTime(filePath).Date < cutoffDate;
        }
    }
}
