using System;
using System.IO;

namespace IndustrialBackup.Services
{
    public class BackupLogger
    {
        private readonly string _logDirectory;

        public BackupLogger(string appDataFolder)
        {
            _logDirectory = Path.Combine(appDataFolder, "Logs");
            Directory.CreateDirectory(_logDirectory);
        }

        public void Write(string message)
        {
            string filePath = Path.Combine(_logDirectory, $"backup_{DateTime.Now:yyyyMMdd}.log");
            string formatted = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
            File.AppendAllText(filePath, formatted);
        }
    }
}