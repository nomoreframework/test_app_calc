using Calculator.Models.LogModels;
using Microsoft.EntityFrameworkCore;

namespace Calculator.DB.DBContext
{
    class LogDBContext : DbContext
    {

        public DbSet<LogModel> OpErrors { get; set; } = null!;
        public DbSet<LogModel> SysErrors { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=calculator_logs.db");
        }
    }
}
