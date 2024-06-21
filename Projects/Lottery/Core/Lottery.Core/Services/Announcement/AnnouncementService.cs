using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Announcement;
using Lottery.Core.Enums;
using Lottery.Core.Models.Announcement.CreateAnnouncement;
using Lottery.Core.Models.Announcement.GetAnnouncementByType;
using Lottery.Core.Models.Announcement.GetUnreadAnnouncements;
using Lottery.Core.Models.Announcement.UpdateAnnouncement;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Announcement;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Announcement
{
    public class AnnouncementService : LotteryBaseService<AnnouncementService>, IAnnouncementService
    {
        public AnnouncementService(
            ILogger<AnnouncementService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            
        }
        public async Task CreateAnnouncement(CreateAnnouncementModel model)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            await announcementRepos.AddAsync(new Data.Entities.Announcement
            {
                Level = model.AnnouncementLevel,
                Type = model.AnnouncementType,
                Content = model.AnnouncementContent,
                CreatedBy = clientAgent.AgentId,
                CreatedAt = ClockService.GetUtcNow()
            });

            await LotteryUow.SaveChangesAsync();
        }

        public async Task DeleteAnnouncement(long id)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var targetAnnouncement = await announcementRepos.FindByIdAsync(id) ?? throw new NotFoundException();
            announcementRepos.Delete(targetAnnouncement);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task<GetAnnouncementByTypeResult> GetAnnouncementByType(GetAnnouncementByTypeModel query)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var agentAnnouncementRepos = LotteryUow.GetRepository<IAgentAnnouncementRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            var announcementQuery = announcementRepos.FindQueryBy(x => x.Type == query.Type).OrderByDescending(x => x.CreatedAt).AsQueryable();

            var result = await announcementRepos.PagingByAsync(announcementQuery, query.PageIndex, query.PageSize);

            var announcements = result.Items.Select(x => new AnnouncementDto
            {
                Id = x.Id,
                Type = x.Type,
                Level = x.Level,
                Content = x.Content,
                CreatedAt = x.CreatedAt
            }).ToList();

            //Save lastest announcement being read of client agent
            if(announcements != null && announcements.Any())
            {
                var existAgentAnnouncement = await agentAnnouncementRepos.FindQuery().FirstOrDefaultAsync(x => x.AgentId == clientAgent.AgentId && x.AnnouncementType == query.Type);
                if(existAgentAnnouncement != null)
                {
                    existAgentAnnouncement.AnnouncementId = announcements.FirstOrDefault().Id;
                }
                else
                {
                    await agentAnnouncementRepos.AddAsync(new Data.Entities.AgentAnnouncement
                    {
                        AgentId = clientAgent.AgentId,
                        AnnouncementId = announcements.FirstOrDefault().Id,
                        AnnouncementType = query.Type,
                        CreatedBy = clientAgent.AgentId,
                        CreatedAt = ClockService.GetUtcNow()
                    });
                }

                await LotteryUow.SaveChangesAsync();
            }
            
            return new GetAnnouncementByTypeResult
            {
                Announcements = announcements,
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }

        public async Task<GetUnreadAnnouncementsResult> GetUnreadAnnouncements()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var agentAnnouncementRepos = LotteryUow.GetRepository<IAgentAnnouncementRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();

            var unreadAnnouncements = new List<UnreadAnnouncement>();
            foreach(var targetType in Enum.GetValues<AnnouncementType>())
            {
                var lastestAgentAnnouncement = await agentAnnouncementRepos.FindQuery().FirstOrDefaultAsync(x => x.AgentId == clientAgent.AgentId && x.AnnouncementType == targetType.ToInt());
                if (lastestAgentAnnouncement is null) continue;
                unreadAnnouncements.Add(new UnreadAnnouncement
                {
                    Type = targetType.ToInt(),
                    Quantity = await announcementRepos.FindQueryBy(x => x.Type == targetType.ToInt() && x.Id > lastestAgentAnnouncement.AnnouncementId).CountAsync()
                });
            }

            return new GetUnreadAnnouncementsResult
            {
                UnreadAnnouncements = unreadAnnouncements
            };
        }

        public async Task UpdateAnnouncement(UpdateAnnouncementModel updateModel)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var targetAnnouncement = await announcementRepos.FindByIdAsync(updateModel.AnnouncementId) ?? throw new NotFoundException();
            targetAnnouncement.Level = updateModel.Level;
            targetAnnouncement.Content = updateModel.Content;

            await LotteryUow.SaveChangesAsync();
        }
    }
}
