using PomodoroFocus.Models;

namespace PomodoroFocus.Services
{
    public interface ISettingsService
    {
        AppSettings CurrentSettings { get; }
        void LoadSettings();
        Task SaveSettingsAsync();
    }
}
