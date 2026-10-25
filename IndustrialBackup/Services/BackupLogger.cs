using System;
using System.IO;
using System.Linq;

namespace IndustrialBackup.Services
{
    public class BackupLogger
    {
        private const int MaxLogLines = 1000; // 유지할 최대 로그 라인 수
        private readonly string _logFilePath;
        private readonly object _syncRoot = new();

        public BackupLogger(string appDataFolder)
        {
            string logDirectory = Path.Combine(appDataFolder, "Logs");
            _logFilePath = Path.Combine(logDirectory, "backup.log");

            try
            {
                Directory.CreateDirectory(logDirectory);
                TrimLogs(); // 프로그램 시작 시 1,000개 초과분 정리
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
                    string directory = Path.GetDirectoryName(_logFilePath)!;
                    Directory.CreateDirectory(directory);

                    string formatted = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

                    // 기존 로그 파일이 있으면 읽어오고, 없으면 새로 생성
                    var lines = File.Exists(_logFilePath)
                        ? File.ReadAllLines(_logFilePath).ToList()
                        : [];

                    lines.Add(formatted);

                    // 최신 1,000개 라인만 남기고 초과분 자르기
                    if (lines.Count > MaxLogLines)
                    {
                        lines = lines.Skip(lines.Count - MaxLogLines).ToList();
                    }

                    File.WriteAllLines(_logFilePath, lines);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private void TrimLogs()
        {
            lock (_syncRoot)
            {
                if (!File.Exists(_logFilePath))
                    return;

                try
                {
                    var lines = File.ReadAllLines(_logFilePath);
                    if (lines.Length > MaxLogLines)
                    {
                        var trimmedLines = lines.Skip(lines.Length - MaxLogLines).ToArray();
                        File.WriteAllLines(_logFilePath, trimmedLines);
                    }
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
}