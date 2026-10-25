using System.Collections.Generic;

namespace IndustrialBackup.Helpers
{
    public static class UiStrings
    {
        public const string Korean = "ko";
        public const string English = "en";

        public static string Current { get; private set; } = Korean;

        private static readonly Dictionary<string, Dictionary<string, string>> Catalog = new()
        {
            [Korean] = new()
            {
                ["WindowTitle"] = "자동 폴더 동기화",
                ["AppTitle"] = "자동 폴더 동기화",
                ["AppSubtitle"] = "지정한 원본 폴더의 데이터를 백업 폴더로 자동 동기화하여 안전하게 보호합니다.",
                ["PathSection"] = "폴더 경로 설정",
                ["SourceLabel"] = "동기화 원본",
                ["TargetLabel"] = "동기화 대상",
                ["Browse"] = "찾아보기",
                ["SettingsSection"] = "백업 환경 설정",
                ["AutoBackup"] = "자동 백업 실행",
                ["Startup"] = "윈도우 시작 시 자동 실행 (시작프로그램)",
                ["IntervalLabel"] = "백업 주기",
                ["LanguageLabel"] = "언어",
                ["Interval1"] = "1분",
                ["Interval10"] = "10분",
                ["Interval30"] = "30분",
                ["Interval60"] = "1시간",
                ["StatusIdle"] = "상태: 대기 중",
                ["StatusRunning"] = "상태: 백업 실행 중",
                ["StatusWaitingNext"] = "상태: 대기 중 (다음 백업까지 {0})",
                ["StartBackup"] = "백업 시작",
                ["StopBackup"] = "백업 중지",
                ["LogSection"] = "실시간 백업 로그",
                ["Footer"] = "Auto Folder Sync v1.0 | 원본 ➔ 대상 폴더 미러링",
                ["BrowseSource"] = "백업할 원본 폴더를 선택하세요.",
                ["BrowseTarget"] = "백업 대상 폴더를 선택하세요.",
                ["NeedBothPaths"] = "원본 경로와 백업 경로를 모두 선택해야 합니다.",
                ["SourceMissing"] = "원본 폴더가 존재하지 않습니다.",
                ["SamePaths"] = "원본 경로와 백업 경로는 같을 수 없습니다.",
                ["StartFailed"] = "백업 시작 실패: {0}",
                ["StopFailed"] = "백업 중지 실패: {0}",
                ["ErrorTitle"] = "오류",
                ["LogInit"] = "시스템 초기화 완료",
                ["LogSourceSelected"] = "원본 경로 선택: {0}",
                ["LogTargetSelected"] = "백업 경로 선택: {0}",
                ["LogBackupStart"] = "백업 시작",
                ["LogBackupStop"] = "백업 중지",
                ["LogStartupOn"] = "시작프로그램 등록됨",
                ["LogStartupOff"] = "시작프로그램 등록 취소됨",
                ["LogIntervalChanged"] = "백업 주기 변경: {0}"
            },
            [English] = new()
            {
                ["WindowTitle"] = "Auto Folder Sync",
                ["AppTitle"] = "Auto Folder Sync",
                ["AppSubtitle"] = "Automatically synchronizes data from the specified source folder to the backup folder to keep it safe.",
                ["PathSection"] = "Folder paths",
                ["SourceLabel"] = "Sync Source",
                ["TargetLabel"] = "Sync Target",
                ["Browse"] = "Browse",
                ["SettingsSection"] = "Backup settings",
                ["AutoBackup"] = "Run automatic backup",
                ["Startup"] = "Start with Windows (startup)",
                ["IntervalLabel"] = "Interval",
                ["LanguageLabel"] = "Language",
                ["Interval1"] = "1 min",
                ["Interval10"] = "10 min",
                ["Interval30"] = "30 min",
                ["Interval60"] = "1 hour",
                ["StatusIdle"] = "Status: Idle",
                ["StatusRunning"] = "Status: Backup in progress",
                ["StatusWaitingNext"] = "Status: Waiting (next backup in {0})",
                ["StartBackup"] = "Start backup",
                ["StopBackup"] = "Stop backup",
                ["LogSection"] = "Live backup log",
                ["Footer"] = "Auto Folder Sync v1.0 | Source ➔ Target mirroring",
                ["BrowseSource"] = "Select the source folder to back up.",
                ["BrowseTarget"] = "Select the backup destination folder.",
                ["NeedBothPaths"] = "Both the source and backup paths are required.",
                ["SourceMissing"] = "The source folder does not exist.",
                ["SamePaths"] = "The source and backup paths cannot be the same.",
                ["StartFailed"] = "Failed to start backup: {0}",
                ["StopFailed"] = "Failed to stop backup: {0}",
                ["ErrorTitle"] = "Error",
                ["LogInit"] = "System initialized",
                ["LogSourceSelected"] = "Source path selected: {0}",
                ["LogTargetSelected"] = "Backup path selected: {0}",
                ["LogBackupStart"] = "Backup started",
                ["LogBackupStop"] = "Backup stopped",
                ["LogStartupOn"] = "Added to Windows startup",
                ["LogStartupOff"] = "Removed from Windows startup",
                ["LogIntervalChanged"] = "Backup interval changed: {0}"
            }
        };

        public static void SetLanguage(string language)
        {
            Current = language == English ? English : Korean;
        }

        public static string T(string key)
        {
            if (Catalog.TryGetValue(Current, out Dictionary<string, string>? table)
                && table.TryGetValue(key, out string? value))
            {
                return value;
            }

            return key;
        }

        public static string Format(string key, params object[] args)
        {
            return string.Format(T(key), args);
        }

        public static string IntervalLabel(int minutes)
        {
            return minutes switch
            {
                1 => T("Interval1"),
                10 => T("Interval10"),
                30 => T("Interval30"),
                60 => T("Interval60"),
                _ => minutes.ToString()
            };
        }
    }
}
