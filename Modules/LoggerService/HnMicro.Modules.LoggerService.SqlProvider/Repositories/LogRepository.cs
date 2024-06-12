using HnMicro.Modules.EntityFrameworkCore.Repositories;
using HnMicro.Modules.LoggerService.SqlProvider.Data;

namespace HnMicro.Modules.LoggerService.SqlProvider.Repositories
{
    public class LogRepository : EntityFrameworkCoreRepository<long, Entities.LogEntry, LoggerContext>, ILogRepository
    {
        public LogRepository(LoggerContext context) : base(context)
        {
        }
    }
}
