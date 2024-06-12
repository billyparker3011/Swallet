using HnMicro.Framework.Logger.Models;
using HnMicro.Modules.LoggerService.SqlProvider.Repositories;
using HnMicro.Modules.LoggerService.SqlProvider.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HnMicro.Modules.LoggerService.Providers
{
    public class SqlProvider : AbstractProvider
    {
        private readonly ILogger<SqlProvider> _logger;

        public SqlProvider(IServiceProvider serviceProvider, IConfiguration configuration) : base(serviceProvider, configuration)
        {
            _logger = serviceProvider.GetService<ILogger<SqlProvider>>();
        }

        protected override void InternalProcess(List<LogModel> messages)
        {
            if (messages.Count == 0) return;

            try
            {
                using var scope = ServiceProvider.CreateScope();
                var uow = scope.ServiceProvider.GetService<ISqlProviderUow>();
                var logRepository = uow.GetRepository<ILogRepository>();
                foreach (var item in messages)
                {
                    logRepository.Add(new LoggerService.SqlProvider.Entities.LogEntry
                    {
                        CategoryName = item.CategoryName,
                        CreatedAt = item.CreatedAt,
                        CreatedBy = item.CreatedBy,
                        Message = item.Message,
                        RoleId = item.RoleId,
                        ServiceCode = item.ServiceCode,
                        ServiceName = item.ServiceName,
                        Stacktrace = item.Stacktrace,
                        Version = item.Version
                    });
                }
                uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
            }
        }
    }
}
