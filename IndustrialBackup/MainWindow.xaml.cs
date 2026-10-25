using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using IndustrialBackup.Models;
using IndustrialBackup.Services;
using Microsoft.Win32;

namespace IndustrialBackup
{
    public partial class MainWindow : Window
    {
        private readonly BackupSettings _settings = new();
        private FolderMirrorService? _backupService;
        private bool _isRunning;

        public MainWindow()
        {
            InitializeComponent();

            // 기본값
            _settings.SourcePath = @"C:\FactoryData";
            _settings.TargetPath = @"D:\FactoryData_Backup";
            _settings.AutoBackupEnabled = true;
            _settings.StartWithWindows = false;
            _settings.BackupIntervalMinutes = 10;

            txtSourcePath.Text = _settings.SourcePath;
            txtTargetPath.Text = _settings.TargetPath;
            chkAutoBackup.IsChecked = _settings.AutoBackupEnabled;
            chkStartup.IsChecked = _settings.StartWithWindows;

            btnBrowseSource.Click += BtnBrowseSource_Click;
            btnBrowseTarget.Click += BtnBrowseTarget_Click;
            btnToggleBackup.Click += BtnToggleBackup_Click;
            chkStartup.Click += ChkStartup_Click;

            Log("시스템 초기화 완료");
        }

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "백업할 원본 폴더를 선택하세요."
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settings.SourcePath = dialog.SelectedPath;
                txtSourcePath.Text = _settings.SourcePath;
                Log($"원본 경로 선택: {_settings.SourcePath}");
            }
        }

        private void BtnBrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "백업 대상 폴더를 선택하세요."
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settings.TargetPath = dialog.SelectedPath;
                txtTargetPath.Text = _settings.TargetPath;
                Log($"백업 경로 선택: {_settings.TargetPath}");
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

        private void ChkStartup_Click(object sender, RoutedEventArgs e)
        {
            _settings.StartWithWindows = chkStartup.IsChecked == true;
            SetStartupRegistration(_settings.StartWithWindows);
            Log(_settings.StartWithWindows
                ? "시작프로그램 등록됨"
                : "시작프로그램 등록 취소됨");
        }

        private void StartBackup()
        {
            try
            {
                _settings.SourcePath = txtSourcePath.Text.Trim();
                _settings.TargetPath = txtTargetPath.Text.Trim();
                _settings.AutoBackupEnabled = chkAutoBackup.IsChecked == true;

                if (string.IsNullOrWhiteSpace(_settings.SourcePath) ||
                    string.IsNullOrWhiteSpace(_settings.TargetPath))
                {
                    MessageBox.Show("원본 경로와 백업 경로를 모두 선택해야 합니다.");
                    return;
                }

                if (!Directory.Exists(_settings.SourcePath))
                {
                    MessageBox.Show("원본 폴더가 존재하지 않습니다.");
                    return;
                }

                if (string.Equals(
                    Path.GetFullPath(_settings.SourcePath).TrimEnd('\\'),
                    Path.GetFullPath(_settings.TargetPath).TrimEnd('\\'),
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("원본 경로와 백업 경로는 같을 수 없습니다.");
                    return;
                }

                _backupService = new FolderMirrorService(
                    _settings.SourcePath,
                    _settings.TargetPath,
                    _settings.BackupIntervalMinutes);

                _backupService.Log += (_, message) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        txtLog.AppendText(message + Environment.NewLine);
                        txtLog.ScrollToEnd();
                    });
                };

                _backupService.Start();

                _isRunning = true;
                btnToggleBackup.Content = "백업 중지";
                txtStatus.Text = "상태: 백업 실행 중";
                statusIndicator.Fill = Brushes.Orange;

                Log("백업 시작");
            }
            catch (Exception ex)
            {
                MessageBox.Show("백업 시작 실패: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopBackup()
        {
            try
            {
                _backupService?.Stop();
                _isRunning = false;

                btnToggleBackup.Content = "백업 시작";
                txtStatus.Text = "상태: 대기 중";
                statusIndicator.Fill = Brushes.Green;

                Log("백업 중지");
            }
            catch (Exception ex)
            {
                MessageBox.Show("백업 중지 실패: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Log(string message)
        {
            txtLog.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            txtLog.ScrollToEnd();
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