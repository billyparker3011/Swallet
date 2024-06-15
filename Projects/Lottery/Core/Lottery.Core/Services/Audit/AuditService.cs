using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Audit;
using Lottery.Core.Repositories.Agent;
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

        public async Task<GetAuditsByTypeResult> GetAuditsByType(GetAuditsByTypeModel query)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var auditRepos = LotteryUow.GetRepository<IAuditRepository>();
            var targetAgentId = ClientContext.Agent.ParentId != 0 ? ClientContext.Agent.ParentId : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();

            IQueryable<Data.Entities.Audit> auditQuery = auditRepos.FindQueryBy(x => x.Type == query.Type && x.CreatedAt >= query.DateFrom.Date && x.CreatedAt <= query.DateTo.AddDays(1).AddTicks(-1));
            if (clientAgent.RoleId != Role.Company.ToInt())
            {
                switch (clientAgent.RoleId)
                {
                    case (int)Role.Supermaster:
                        auditQuery = auditQuery.Where(x => x.SupermasterId == clientAgent.AgentId);
                        break;
                    case (int)Role.Master:
                        auditQuery = auditQuery.Where(x => x.MasterId == clientAgent.AgentId);
                        break;
                    case (int)Role.Agent:
                        auditQuery = auditQuery.Where(x => x.AgentId == clientAgent.AgentId);
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                auditQuery = auditQuery.Where(x =>
                        x.UserName.Contains(query.SearchTerm) ||
                        x.LastName.Contains(query.SearchTerm) ||
                        x.FirstName.Contains(query.SearchTerm));
            }

            var result = await auditRepos.PagingByAsync(auditQuery, query.PageIndex, query.PageSize);
            return new GetAuditsByTypeResult
            {
                Audits = result.Items.Select(x => new AuditDto
                {
                    AuditId = x.AuditId,
                    Action = x.Action,
                    AuditData = x.AuditData,
                    CreatedAt = x.CreatedAt,
                    EditedBy = x.EdittedBy,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Type = x.Type,
                    Username = x.UserName,
                    AuditSettingDatas = string.IsNullOrEmpty(query.SearchBetKind) ? x.AuditSettingDatas.OrderBy(p => p.BetKind).ToList() 
                                                                                  : x.AuditSettingDatas.Where(x => x.BetKind == query.SearchBetKind).OrderBy(x => x.BetKind).ToList()
                }),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
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
                AgentId = auditParams.AgentId,
                AuditSettingDatas = auditParams.AuditSettingDatas
            });

            await LotteryUow.SaveChangesAsync();
        }
    }
}
