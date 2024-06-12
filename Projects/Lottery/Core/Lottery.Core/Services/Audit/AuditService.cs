using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Helpers;
using Lottery.Core.Repositories.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Audit
{
    public class AuditService : LotteryBaseService<AuditService>, IAuditService
    {
        public AuditService(
            ILogger<AuditService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) 
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task SaveAuditData(AuditParams auditParams)
        {
            var auditRepository = LotteryUow.GetRepository<IAuditRepository>();

            var clientInformation = ClientContext.GetClientInformation();

            await auditRepository.AddAsync(new Data.Entities.Audit
            {
                Type = auditParams.Type,
                Action = auditParams.Action,
                UserName = auditParams.AgentUserName,
                FirstName = auditParams.AgentFirstName,
                LastName = auditParams.AgentLastName,
                AuditData = new AuditData
                {
                    Domain = clientInformation?.Domain,
                    Browser = clientInformation?.Browser,
                    Ip = clientInformation?.IpAddress,
                    Country = AuditDataHelper.DefaultCountry,
                    Detail = auditParams.DetailMessage,
                    OldValue = auditParams.OldValue,
                    NewValue = auditParams.NewValue
                },
                CreatedAt = ClockService.GetUtcNow(),
                EdittedBy = auditParams.EditedUsername,
                SupermasterId = auditParams.SupermasterId,
                MasterId = auditParams.MasterId,
                AgentId = auditParams.AgentId
            });

            await LotteryUow.SaveChangesAsync();
        }
    }
}
