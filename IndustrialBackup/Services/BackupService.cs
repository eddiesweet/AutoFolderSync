using System;
using System.Collections.Generic;
using System.IO;

namespace IndustrialBackup.Services
{
    public class BackupService
    {
        private readonly BackupLogger _logger;
        private readonly object _syncRoot = new();

        public event EventHandler<string>? Log;

        public BackupService(BackupLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Mirror(string sourcePath, string targetPath, CancellationToken cancellationToken = default)
        {
            lock (_syncRoot)
            {
                WriteLog($"복사 시작: {sourcePath} -> {targetPath}");

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (string.IsNullOrWhiteSpace(sourcePath) || string.IsNullOrWhiteSpace(targetPath))
                    {
                        WriteLog("복사 실패: 원본 경로 또는 대상 경로가 비어 있습니다.");
                        return;
                    }

                    if (!Directory.Exists(sourcePath))
                    {
                        WriteLog($"복사 실패: 원본 폴더가 없습니다. ({sourcePath})");
                        return;
                    }

                    string fullSource = Path.GetFullPath(sourcePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    string fullTarget = Path.GetFullPath(targetPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    if (string.Equals(fullSource, fullTarget, StringComparison.OrdinalIgnoreCase))
                    {
                        WriteLog("복사 실패: 원본 경로와 대상 경로가 같습니다.");
                        return;
                    }

                    Directory.CreateDirectory(fullTarget);

                    int copied = 0;
                    int skipped = 0;
                    int failed = 0;
                    int removed = 0;
                    var sourceRelativePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (string sourceFile in Directory.GetFiles(fullSource, "*", SearchOption.AllDirectories))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        string relativePath = Path.GetRelativePath(fullSource, sourceFile);
                        sourceRelativePaths.Add(relativePath);
                        string destFile = Path.Combine(fullTarget, relativePath);

                        try
                        {
                            string? destDirectory = Path.GetDirectoryName(destFile);
                            if (!string.IsNullOrEmpty(destDirectory))
                                Directory.CreateDirectory(destDirectory);

                            if (IsUpToDate(sourceFile, destFile))
                            {
                                skipped++;
                                continue;
                            }

                            File.Copy(sourceFile, destFile, overwrite: true);
                            File.SetLastWriteTimeUtc(destFile, File.GetLastWriteTimeUtc(sourceFile));
                            copied++;
                            WriteLog($"복사 성공: {relativePath}");
                        }
                        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                        {
                            failed++;
                            WriteLog($"복사 실패: {relativePath} ({ex.Message})");
                        }
                    }

                    foreach (string destFile in Directory.GetFiles(fullTarget, "*", SearchOption.AllDirectories))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        string relativePath = Path.GetRelativePath(fullTarget, destFile);
                        if (sourceRelativePaths.Contains(relativePath))
                            continue;

                        try
                        {
                            File.Delete(destFile);
                            removed++;
                            WriteLog($"미러 삭제: {relativePath}");
                        }
                        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                        {
                            failed++;
                            WriteLog($"미러 삭제 실패: {relativePath} ({ex.Message})");
                        }
                    }

                    WriteLog($"복사 완료: 성공 {copied}건, 건너뜀 {skipped}건, 삭제 {removed}건, 실패 {failed}건");
                }
                catch (OperationCanceledException)
                {
                    WriteLog("백업이 중지되었습니다.");
                }
                catch (Exception ex)
                {
                    WriteLog($"예외 발생: {ex.GetType().Name} - {ex.Message}");
                }
            }
        }

        private void WriteLog(string message)
        {
            _logger.Write(message);
            Log?.Invoke(this, message);
        }

        private static bool IsUpToDate(string sourceFile, string destFile)
        {
            if (!File.Exists(destFile))
                return false;

            var sourceInfo = new FileInfo(sourceFile);
            var destInfo = new FileInfo(destFile);

            return sourceInfo.Length == destInfo.Length
                && sourceInfo.LastWriteTimeUtc == destInfo.LastWriteTimeUtc;
        }
    }
}
