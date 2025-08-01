using Microsoft.EntityFrameworkCore;
using PomodoroFocus.Models;

namespace PomodoroFocus.Data
{
    public class AppDbContext : DbContext
    {
        // 这个 DbSet<PomodoroSession> 对应了数据库中将被创建的一张名为 "Sessions" 的表
        public DbSet<PomodoroSession> Sessions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
