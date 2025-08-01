using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PomodoroFocus.Data;
using PomodoroFocus.Services;
#if WINDOWS
using PomodoroFocus.Platforms.Windows.Services;
#endif

namespace PomodoroFocus
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IPomodoroTimerService, PomodoroTimerService>();
            builder.Services.AddSingleton<ILLMService, LLMService>();
            builder.Services.AddSingleton<IThemeService, ThemeService>();
            builder.Services.AddTransient<WidgetPage>();

#if WINDOWS
            builder.Services.AddSingleton<IFloatingWindowService, WindowsFloatingWindowService>();
            builder.Services.AddSingleton<IWindowActivationService, WindowsActivationService>();
#endif

            // 构造 SQLite 数据库的完整路径
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "pomodoro.db");

            // 注册数据库上下文服务
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}")
            );

            builder.Services.AddTransient<DatabaseInitializer>();

            var app = builder.Build();

            // 从服务容器中获取初始化服务实例
            var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
            // 执行初始化（也就是迁移）
            dbInitializer.Initialize();

            return app;
        }
    }
}
