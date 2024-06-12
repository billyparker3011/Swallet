using HnMicro.Modules.LoggerService.SqlProvider.Entities;
using Microsoft.EntityFrameworkCore;

namespace HnMicro.Modules.LoggerService.SqlProvider.Data
{
    public class LoggerContext : DbContext
    {
        public LoggerContext(DbContextOptions options) : base(options)
        {

        }

        public virtual DbSet<LogEntry> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
