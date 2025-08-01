using PomodoroFocus.Models;
using System.Text.Json;

namespace PomodoroFocus.Services
{
    public class SettingsService : ISettingsService
    {
        private const string SettingsFileName = "appsettings.json";
        private readonly string _settingsFilePath;

        public AppSettings CurrentSettings { get; private set; }

        public SettingsService()
        {
            // FileSystem.AppDataDirectory 是 .NET MAUI 提供的标准、跨平台的应用数据存储位置
            _settingsFilePath = Path.Combine(FileSystem.AppDataDirectory, SettingsFileName);
            LoadSettings();
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    // 使用 ?? 来确保即使文件内容为空或反序列化失败，也有一个默认对象
                    CurrentSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    CurrentSettings = new AppSettings();
                }
            }
            catch
            {
                // 如果发生任何IO或反序列化错误，回退到默认设置
                CurrentSettings = new AppSettings();
            }
        }

        public async Task SaveSettingsAsync()
        {
            var json = JsonSerializer.Serialize(CurrentSettings);
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
    }
}
