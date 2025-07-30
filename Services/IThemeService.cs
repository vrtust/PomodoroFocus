using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroFocus.Services
{
    public interface IThemeService
    {
        Task SetThemeAsync(string theme, IJSRuntime jsRuntime);
    }
}
