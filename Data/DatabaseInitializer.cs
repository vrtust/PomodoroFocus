using Microsoft.EntityFrameworkCore;

namespace PomodoroFocus.Data
{
    public class DatabaseInitializer
    {
        private readonly AppDbContext _context;

        // 依赖注入，获取数据库上下文
        public DatabaseInitializer(AppDbContext context)
        {
            _context = context;
        }

        // 一个公开的方法，用于执行迁移
        public void Initialize()
        {
            try
            {
                // 这是 Database.Migrate() 的同步版本
                _context.Database.Migrate();
            }
            catch (Exception ex)
            {
                // 在这里可以添加日志记录，方便排查未来的问题
                Console.WriteLine($"数据库迁移失败: {ex.Message}");
                throw;
            }
        }
    }
}
