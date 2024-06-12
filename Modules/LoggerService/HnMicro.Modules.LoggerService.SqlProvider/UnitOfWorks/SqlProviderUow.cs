using HnMicro.Modules.EntityFrameworkCore.UnitOfWorks;
using HnMicro.Modules.LoggerService.SqlProvider.Data;

namespace HnMicro.Modules.LoggerService.SqlProvider.UnitOfWorks
{
    public class SqlProviderUow : EntityFrameworkCoreUnitOfWork<LoggerContext>, ISqlProviderUow
    {
        public SqlProviderUow(LoggerContext context) : base(context)
        {
        }
    }
}
