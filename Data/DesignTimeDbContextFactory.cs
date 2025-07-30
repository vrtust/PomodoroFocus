using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PomodoroFocus.Data;

// 这个类是专门为 EF Core 设计时工具（如 Add-Migration）准备的。
// 它告诉工具在没有完整应用环境的情况下，如何创建 AppDbContext。
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // 关键点：在设计时，我们不需要复杂的 FileSystem.AppDataDirectory。
        // 我们只需要一个临时的、有效的数据库连接字符串，让工具能够分析模型即可。
        // 工具会用这个连接字符串在你的项目根目录创建一个临时的数据库文件来分析，
        // 这个文件与最终用户设备上的数据库无关。
        optionsBuilder.UseSqlite("Data Source=design_time.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
