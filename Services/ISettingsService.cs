using PomodoroFocus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroFocus.Services
{
    public interface ISettingsService
    {
        AppSettings CurrentSettings { get; }
        void LoadSettings();
        Task SaveSettingsAsync();
    }
}
