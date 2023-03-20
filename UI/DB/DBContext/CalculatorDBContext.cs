using Calculator.Models.LogModels;
using Microsoft.EntityFrameworkCore;

namespace Calculator.DB.DBContext
{
    class CalculatorDBContext : DbContext
    {
        public DbSet<LogModel> Operations { get; set; } = null!;
        public DbSet<LogModel> Errors { get; set;} = null!; 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=operations.db");
        }
    }
}
