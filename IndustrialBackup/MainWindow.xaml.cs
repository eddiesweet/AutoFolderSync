using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using IndustrialBackup.Helpers;
using IndustrialBackup.Models;
using IndustrialBackup.Services;
using Microsoft.Win32;

namespace IndustrialBackup
{
    public partial class MainWindow : Window
    {
        private enum RunStatus
        {
            Idle,
            Running,
            WaitingNext
        }

        private static readonly int[] BackupIntervals = { 1, 10, 30, 60 };

        private readonly BackupSettings _settings = new();
        private readonly BackupLogger _logger;
        private readonly BackupService _backupService;
        private CancellationTokenSource? _backupCts;
        private DispatcherTimer? _autoBackupTimer;
        private Task? _backupTask;
        private bool _isRunning;
        private bool _isUpdatingUi;
        private RunStatus _runStatus = RunStatus.Idle;

        public MainWindow()
        {
            InitializeComponent();

            string appDataFolder = Path.GetDirectoryName(BackupSettings.GetSettingsPath())
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IndustrialBackup");
            _logger = new BackupLogger(appDataFolder);
            _backupService = new BackupService(_logger);
            _backupService.Log += BackupService_Log;

            _settings.SourcePath = @"C:\source";
            _settings.TargetPath = @"D:\databackup";
            _settings.AutoBackupEnabled = true;
            _settings.StartWithWindows = false;
            _settings.BackupIntervalMinutes = 10;

            txtSourcePath.Text = _settings.SourcePath;
            txtTargetPath.Text = _settings.TargetPath;
            chkAutoBackup.IsChecked = _settings.AutoBackupEnabled;
            chkStartup.IsChecked = _settings.StartWithWindows;

            UiStrings.SetLanguage(UiStrings.Korean);
            FillIntervalOptions();
            ApplyLanguage();

            btnBrowseSource.Click += BtnBrowseSource_Click;
            btnBrowseTarget.Click += BtnBrowseTarget_Click;
            chkStartup.Click += ChkStartup_Click;
            Closed += MainWindow_Closed;

            AppendLog(UiStrings.T("LogInit"));
        }

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = UiStrings.T("BrowseSource")
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settings.SourcePath = dialog.SelectedPath;
                txtSourcePath.Text = _settings.SourcePath;
                AppendLog(UiStrings.Format("LogSourceSelected", _settings.SourcePath));
            }
        }

        private void BtnBrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = UiStrings.T("BrowseTarget")
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settings.TargetPath = dialog.SelectedPath;
                txtTargetPath.Text = _settings.TargetPath;
                AppendLog(UiStrings.Format("LogTargetSelected", _settings.TargetPath));
            }
        }

        private void BtnToggleBackup_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                StopBackup();
                return;
            }

            StartBackup();
        }

        private void CmbInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingUi || cmbInterval.SelectedItem is not ComboBoxItem item || item.Tag is not int minutes)
                return;

            _settings.BackupIntervalMinutes = minutes;
            ApplyBackupIntervalToTimer();
            RefreshStatusText();
            AppendLog(UiStrings.Format("LogIntervalChanged", UiStrings.IntervalLabel(minutes)));
        }

        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingUi || cmbLanguage.SelectedItem is not ComboBoxItem item)
                return;

            string language = item.Tag?.ToString() ?? UiStrings.Korean;
            UiStrings.SetLanguage(language);
            ApplyLanguage();
        }

        private void ChkStartup_Click(object sender, RoutedEventArgs e)
        {
            _settings.StartWithWindows = chkStartup.IsChecked == true;
            SetStartupRegistration(_settings.StartWithWindows);
            AppendLog(_settings.StartWithWindows
                ? UiStrings.T("LogStartupOn")
                : UiStrings.T("LogStartupOff"));
        }

        private void StartBackup()
        {
            try
            {
                _settings.SourcePath = txtSourcePath.Text.Trim();
                _settings.TargetPath = txtTargetPath.Text.Trim();
                _settings.AutoBackupEnabled = chkAutoBackup.IsChecked == true;
                _settings.BackupIntervalMinutes = ReadSelectedIntervalMinutes();

                if (string.IsNullOrWhiteSpace(_settings.SourcePath) ||
                    string.IsNullOrWhiteSpace(_settings.TargetPath))
                {
                    MessageBox.Show(UiStrings.T("NeedBothPaths"));
                    return;
                }

                if (!Directory.Exists(_settings.SourcePath))
                {
                    MessageBox.Show(UiStrings.T("SourceMissing"));
                    return;
                }

                if (string.Equals(
                    Path.GetFullPath(_settings.SourcePath).TrimEnd('\\'),
                    Path.GetFullPath(_settings.TargetPath).TrimEnd('\\'),
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(UiStrings.T("SamePaths"));
                    return;
                }

                _backupCts = new CancellationTokenSource();
                _isRunning = true;
                SetRunStatus(RunStatus.Running);
                AppendLog(UiStrings.T("LogBackupStart"));

                string sourcePath = _settings.SourcePath;
                string targetPath = _settings.TargetPath;
                CancellationToken token = _backupCts.Token;

                _backupTask = Task.Run(() => _backupService.Mirror(sourcePath, targetPath, token))
                    .ContinueWith(_ => OnBackupFinished(), TaskScheduler.FromCurrentSynchronizationContext());

                if (_settings.AutoBackupEnabled)
                    StartAutoBackupTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    UiStrings.Format("StartFailed", ex.Message),
                    UiStrings.T("ErrorTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                ResetIdleUi();
            }
        }

        private void StopBackup()
        {
            try
            {
                StopAutoBackupTimer();
                _backupCts?.Cancel();
                _isRunning = false;
                ResetIdleUi();
                AppendLog(UiStrings.T("LogBackupStop"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    UiStrings.Format("StopFailed", ex.Message),
                    UiStrings.T("ErrorTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RunBackup()
        {
            if (_backupTask is { IsCompleted: false })
                return;

            CancellationToken token = _backupCts?.Token ?? CancellationToken.None;
            string sourcePath = _settings.SourcePath;
            string targetPath = _settings.TargetPath;

            _backupTask = Task.Run(() => _backupService.Mirror(sourcePath, targetPath, token))
                .ContinueWith(_ => OnBackupFinished(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnBackupFinished()
        {
            if (!_isRunning || !IsLoaded)
                return;

            if (_settings.AutoBackupEnabled)
            {
                SetRunStatus(RunStatus.WaitingNext);
                return;
            }

            _isRunning = false;
            ResetIdleUi();
        }

        private void StartAutoBackupTimer()
        {
            StopAutoBackupTimer();
            _autoBackupTimer = new DispatcherTimer
            {
                Interval = GetBackupInterval()
            };
            _autoBackupTimer.Tick += AutoBackupTimer_Tick;
            _autoBackupTimer.Start();
        }

        private void ApplyBackupIntervalToTimer()
        {
            if (_autoBackupTimer is null)
                return;

            _autoBackupTimer.Stop();
            _autoBackupTimer.Interval = GetBackupInterval();
            _autoBackupTimer.Start();
        }

        private TimeSpan GetBackupInterval()
        {
            return TimeSpan.FromMinutes(Math.Max(1, _settings.BackupIntervalMinutes));
        }

        private void AutoBackupTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isRunning)
                return;

            SetRunStatus(RunStatus.Running);
            RunBackup();
        }

        private void StopAutoBackupTimer()
        {
            if (_autoBackupTimer is null)
                return;

            _autoBackupTimer.Tick -= AutoBackupTimer_Tick;
            _autoBackupTimer.Stop();
            _autoBackupTimer = null;
        }

        private void ResetIdleUi()
        {
            SetRunStatus(RunStatus.Idle);
        }

        private void SetRunStatus(RunStatus status)
        {
            _runStatus = status;
            RefreshStatusText();
        }

        private void RefreshStatusText()
        {
            switch (_runStatus)
            {
                case RunStatus.Running:
                    btnToggleBackup.Content = UiStrings.T("StopBackup");
                    txtStatus.Text = UiStrings.T("StatusRunning");
                    statusIndicator.Fill = Brushes.Orange;
                    break;
                case RunStatus.WaitingNext:
                    btnToggleBackup.Content = UiStrings.T("StopBackup");
                    txtStatus.Text = UiStrings.Format("StatusWaitingNext", UiStrings.IntervalLabel(_settings.BackupIntervalMinutes));
                    statusIndicator.Fill = Brushes.Orange;
                    break;
                default:
                    btnToggleBackup.Content = UiStrings.T("StartBackup");
                    txtStatus.Text = UiStrings.T("StatusIdle");
                    statusIndicator.Fill = Brushes.Green;
                    break;
            }
        }

        private void FillIntervalOptions()
        {
            _isUpdatingUi = true;
            try
            {
                int selected = _settings.BackupIntervalMinutes;
                cmbInterval.Items.Clear();

                ComboBoxItem? match = null;
                foreach (int minutes in BackupIntervals)
                {
                    var item = new ComboBoxItem
                    {
                        Content = UiStrings.IntervalLabel(minutes),
                        Tag = minutes
                    };
                    cmbInterval.Items.Add(item);
                    if (minutes == selected)
                        match = item;
                }

                cmbInterval.SelectedItem = match ?? cmbInterval.Items[1];
            }
            finally
            {
                _isUpdatingUi = false;
            }
        }

        private int ReadSelectedIntervalMinutes()
        {
            if (cmbInterval.SelectedItem is ComboBoxItem item && item.Tag is int minutes)
                return minutes;

            return 10;
        }

        private void ApplyLanguage()
        {
            // [추가된 안전장치] 화면 컨트롤이 미처 다 생성되기 전에 이벤트가 발생하면 무시합니다.
            if (txtLogSection == null || txtAppTitle == null)
                return;

            _isUpdatingUi = true;
            try
            {
                Title = UiStrings.T("WindowTitle");
                txtAppTitle.Text = UiStrings.T("AppTitle");
                txtAppSubtitle.Text = UiStrings.T("AppSubtitle");
                txtPathSection.Text = UiStrings.T("PathSection");
                lblSourcePath.Text = UiStrings.T("SourceLabel");
                lblTargetPath.Text = UiStrings.T("TargetLabel");
                btnBrowseSource.Content = UiStrings.T("Browse");
                btnBrowseTarget.Content = UiStrings.T("Browse");
                txtSettingsSection.Text = UiStrings.T("SettingsSection");
                chkAutoBackup.Content = UiStrings.T("AutoBackup");
                chkStartup.Content = UiStrings.T("Startup");
                lblInterval.Text = UiStrings.T("IntervalLabel");
                lblLanguage.Text = UiStrings.T("LanguageLabel");
                txtLogSection.Text = UiStrings.T("LogSection");
                txtFooter.Text = UiStrings.T("Footer");
            }
            finally
            {
                _isUpdatingUi = false;
            }

            FillIntervalOptions();
            RefreshStatusText();
        }
        private void BackupService_Log(object? sender, string message)
        {
            Dispatcher.Invoke(() => AppendLog(message));
        }

        private void AppendLog(string message)
        {
            lstLog.Items.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            if (lstLog.Items.Count > 0)
                lstLog.ScrollIntoView(lstLog.Items[lstLog.Items.Count - 1]);
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            StopAutoBackupTimer();
            _backupCts?.Cancel();
            _backupService.Log -= BackupService_Log;
        }

        private static void SetStartupRegistration(bool enable)
        {
            const string keyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            using var key = Registry.CurrentUser.OpenSubKey(keyName, true) ?? Registry.CurrentUser.CreateSubKey(keyName);

            if (enable)
            {
                key.SetValue("IndustrialBackup", $"\"{appPath}\"");
            }
            else
            {
                key.DeleteValue("IndustrialBackup", false);
            }
        }
    }
}
