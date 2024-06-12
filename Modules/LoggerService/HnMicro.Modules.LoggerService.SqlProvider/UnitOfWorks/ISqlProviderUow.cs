using HnMicro.Modules.EntityFrameworkCore.UnitOfWorks;
using HnMicro.Modules.LoggerService.SqlProvider.Data;

namespace HnMicro.Modules.LoggerService.SqlProvider.UnitOfWorks
{
    public interface ISqlProviderUow : IEntityFrameworkCoreUnitOfWork<LoggerContext>
    {
    }
}
