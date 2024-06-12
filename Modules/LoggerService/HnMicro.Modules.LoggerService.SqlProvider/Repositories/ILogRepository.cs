using HnMicro.Modules.EntityFrameworkCore.Repositories;
using HnMicro.Modules.LoggerService.SqlProvider.Data;

namespace HnMicro.Modules.LoggerService.SqlProvider.Repositories
{
    public interface ILogRepository : IEntityFrameworkCoreRepository<long, Entities.LogEntry, LoggerContext>
    {
    }
}
