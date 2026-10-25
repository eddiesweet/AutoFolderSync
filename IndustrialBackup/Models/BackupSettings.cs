using System;
using System.IO;
using System.Text.Json;

namespace IndustrialBackup.Models
{
    public class BackupSettings
    {
        public string SourcePath { get; set; } = @"C:\Dispenser";
        public string TargetPath { get; set; } = @"D:\Dispenser";
        public bool AutoBackupEnabled { get; set; } = true;
        public bool StartWithWindows { get; set; } = false;
        public int BackupIntervalMinutes { get; set; } = 10;

        public static string GetSettingsPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "IndustrialBackup");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "settings.json");
        }

        public void Save()
        {
            string path = GetSettingsPath();
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(path, json);
        }

        public static BackupSettings Load()
        {
            string path = GetSettingsPath();

            if (!File.Exists(path))
            {
                return new BackupSettings();
            }

            try
            {
                string json = File.ReadAllText(path);
                var settings = JsonSerializer.Deserialize<BackupSettings>(json);

                if (settings is null)
                {
                    return new BackupSettings();
                }

                return settings;
            }
            catch
            {
                return new BackupSettings();
            }
        }
    }
}