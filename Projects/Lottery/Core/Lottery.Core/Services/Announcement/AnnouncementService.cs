using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Dtos.Announcement;
using Lottery.Core.Enums;
using Lottery.Core.Models.Announcement.CreateAnnouncement;
using Lottery.Core.Models.Announcement.GetAnnouncementByType;
using Lottery.Core.Models.Announcement.GetUnreadAnnouncements;
using Lottery.Core.Models.Announcement.UpdateAnnouncement;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Announcement;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            var agentAnnouncementRepos = LotteryUow.GetRepository<IAgentAnnouncementRepository>();
            var playerAnnouncementRepos = LotteryUow.GetRepository<IPlayerAnnouncementRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var announcementCreated = new Data.Entities.Announcement
            {
                Level = model.AnnouncementLevel,
                Type = model.AnnouncementType,
                Content = model.AnnouncementContent,
                CreatedBy = clientAgent.AgentId,
                CreatedAt = ClockService.GetUtcNow()
            };
            await announcementRepos.AddAsync(announcementCreated);
            await LotteryUow.SaveChangesAsync();

            if (model.AnnouncementReceivers.Any() && model.AnnouncementType == AnnouncementType.Private.ToInt())
            {
                var privateAgentAnnouncements = new List<AgentAnnouncement>();
                var privatePlayerAnnouncements = new List<PlayerAnnouncement>();
                model.AnnouncementReceivers.ForEach(item =>
                {
                    if (item.IsAgent)
                    {
                        privateAgentAnnouncements.Add(new AgentAnnouncement
                        {
                            AgentId = item.ReceivedId,
                            AnnouncementId = announcementCreated.Id,
                            AnnouncementType = AnnouncementType.Private.ToInt(),
                            CreatedAt = ClockService.GetUtcNow(),
                            CreatedBy = clientAgent.AgentId
                        });
                    }
                    else
                    {
                        privatePlayerAnnouncements.Add(new PlayerAnnouncement
                        {
                            PlayerId = item.ReceivedId,
                            AnnouncementId = announcementCreated.Id,
                            AnnouncementType = AnnouncementType.Private.ToInt(),
                            CreatedAt = ClockService.GetUtcNow(),
                            CreatedBy = clientAgent.AgentId
                        });
                    }
                });

                if(privateAgentAnnouncements.Count > 0) await agentAnnouncementRepos.AddRangeAsync(privateAgentAnnouncements);
                if(privatePlayerAnnouncements.Count > 0) await playerAnnouncementRepos.AddRangeAsync(privatePlayerAnnouncements);

                await LotteryUow.SaveChangesAsync();
            }
        }

        public async Task DeleteAnnouncement(long id)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var agentAnnouncementRepos = LotteryUow.GetRepository<IAgentAnnouncementRepository>();
            var playerAnnouncementRepos = LotteryUow.GetRepository<IPlayerAnnouncementRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var targetAnnouncement = await announcementRepos.FindByIdAsync(id) ?? throw new NotFoundException();
            if(targetAnnouncement.Type == AnnouncementType.Private.ToInt())
            {
                var privateAgentAnnouncementIds = await agentAnnouncementRepos.FindQueryBy(x => x.AnnouncementId == targetAnnouncement.Id && x.AnnouncementType == AnnouncementType.Private.ToInt()).Select(x => x.Id).ToListAsync();
                agentAnnouncementRepos.DeleteByIds(privateAgentAnnouncementIds);
                var privatePlayerAnnouncementIds = await playerAnnouncementRepos.FindQueryBy(x => x.AnnouncementId == targetAnnouncement.Id && x.AnnouncementType == AnnouncementType.Private.ToInt()).Select(x => x.Id).ToListAsync();
                playerAnnouncementRepos.DeleteByIds(privatePlayerAnnouncementIds);
            }
            announcementRepos.Delete(targetAnnouncement);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task<GetAnnouncementByTypeResult> GetAnnouncementByType(GetAnnouncementByTypeModel query)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var agentAnnouncementRepos = LotteryUow.GetRepository<IAgentAnnouncementRepository>();
            var playerAnnouncementRepos = LotteryUow.GetRepository<IPlayerAnnouncementRepository>();

            var announcements = new List<AnnouncementDto>();
            var announcementMetadata = new HnMicro.Framework.Responses.ApiResponseMetadata();
            if (query.IsAgent)
            {
                (announcements, announcementMetadata) = await GetAgentAnnouncementByType(agentRepos,  agentAnnouncementRepos, announcementRepos, query, playerRepos, playerAnnouncementRepos);
            }
            else
            {
                (announcements, announcementMetadata) = await GetPlayerAnnouncementByType(playerRepos, playerAnnouncementRepos, announcementRepos, query);
            }

            return new GetAnnouncementByTypeResult
            {
                Announcements = announcements,
                Metadata = announcementMetadata
            };
        }

        private async Task<(List<AnnouncementDto>, HnMicro.Framework.Responses.ApiResponseMetadata)> GetPlayerAnnouncementByType(IPlayerRepository playerRepos, IPlayerAnnouncementRepository playerAnnouncementRepos, IAnnouncementRepository announcementRepos, GetAnnouncementByTypeModel query)
        {
            var player = await playerRepos.FindByIdAsync(ClientContext.Player.PlayerId) ?? throw new NotFoundException();
            IQueryable<Data.Entities.Announcement> announcementQuery = announcementRepos.FindQueryBy(x => x.Type == query.Type).OrderByDescending(x => x.CreatedAt).AsQueryable();
            if (query.Type == AnnouncementType.Private.ToInt())
            {
                var announcementIds = await playerAnnouncementRepos.FindQueryBy(x => x.PlayerId == player.PlayerId && x.AnnouncementType == query.Type).Select(x => x.AnnouncementId).ToListAsync();
                announcementQuery = announcementRepos.FindQueryBy(x => announcementIds.Contains(x.Id)).OrderByDescending(x => x.CreatedAt).AsQueryable();
            }
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
            if (announcements != null && announcements.Any() && query.Type != AnnouncementType.Private.ToInt())
            {
                var existPlayerAnnouncement = await playerAnnouncementRepos.FindQuery().FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.AnnouncementType == query.Type);
                if (existPlayerAnnouncement != null)
                {
                    existPlayerAnnouncement.AnnouncementId = announcements.FirstOrDefault().Id;
                }
                else
                {
                    await playerAnnouncementRepos.AddAsync(new Data.Entities.PlayerAnnouncement
                    {
                        PlayerId = player.PlayerId,
                        AnnouncementId = announcements.FirstOrDefault().Id,
                        AnnouncementType = query.Type,
                        CreatedBy = player.PlayerId,
                        CreatedAt = ClockService.GetUtcNow()
                    });
                }

                await LotteryUow.SaveChangesAsync();
            }

            return (announcements, new HnMicro.Framework.Responses.ApiResponseMetadata
            {
                NoOfPages = result.Metadata.NoOfPages,
                NoOfRows = result.Metadata.NoOfRows,
                NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                Page = result.Metadata.Page
            });
        }

        private async Task<(List<AnnouncementDto>, HnMicro.Framework.Responses.ApiResponseMetadata)> GetAgentAnnouncementByType(IAgentRepository agentRepos, IAgentAnnouncementRepository agentAnnouncementRepos, IAnnouncementRepository announcementRepos, GetAnnouncementByTypeModel query, IPlayerRepository playerRepos, IPlayerAnnouncementRepository playerAnnouncementRepos)
        {
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            IQueryable<Data.Entities.Announcement> announcementQuery = announcementRepos.FindQueryBy(x => x.Type == query.Type).OrderByDescending(x => x.CreatedAt).AsQueryable();
            if (query.Type == AnnouncementType.Private.ToInt() && ClientContext.Agent.RoleId != Role.Company.ToInt())
            {
                var announcementIds = await agentAnnouncementRepos.FindQueryBy(x => x.AgentId == clientAgent.AgentId && x.AnnouncementType == query.Type).Select(x => x.AnnouncementId).ToListAsync();
                announcementQuery = announcementRepos.FindQueryBy(x => announcementIds.Contains(x.Id)).OrderByDescending(x => x.CreatedAt).AsQueryable();
            }
            var result = await announcementRepos.PagingByAsync(announcementQuery, query.PageIndex, query.PageSize);

            var announcements = result.Items.Select(x => new AnnouncementDto
            {
                Id = x.Id,
                Type = x.Type,
                Level = x.Level,
                Content = x.Content,
                CreatedAt = x.CreatedAt
            }).ToList();

            if (query.Type == AnnouncementType.Private.ToInt() && ClientContext.Agent.RoleId == Role.Company.ToInt())
            {
                var selectedAgentAnnouncements = await agentAnnouncementRepos.FindQueryBy(x => announcements.Select(f => f.Id).Contains(x.AnnouncementId))
                                            .Join(agentRepos.FindQuery(), x => x.AgentId, y => y.AgentId, (agentAnnouncement, agentInfo) => new
                                            {
                                                agentAnnouncement,
                                                agentInfo
                                            })
                                            .Select(res => new
                                            {
                                                TargetId = res.agentAnnouncement.AgentId,
                                                res.agentAnnouncement.AnnouncementId,
                                                res.agentInfo.Username,
                                                IsAgent = true
                                            })
                                            .ToListAsync();
                var selectedPlayerAnnouncements = await playerAnnouncementRepos.FindQueryBy(x => announcements.Select(f => f.Id).Contains(x.AnnouncementId))
                                            .Join(playerRepos.FindQuery(), x => x.PlayerId, y => y.PlayerId, (playerAnnouncement, playerInfo) => new
                                            {
                                                playerAnnouncement,
                                                playerInfo
                                            })
                                            .Select(res => new
                                            {
                                                TargetId = res.playerAnnouncement.PlayerId,
                                                res.playerAnnouncement.AnnouncementId,
                                                res.playerInfo.Username,
                                                IsAgent = false
                                            })
                                            .ToListAsync();

                var unionSelections = selectedAgentAnnouncements.Concat(selectedPlayerAnnouncements).ToList();

                foreach (var announcement in announcements)
                {
                    announcement.ReceivedAgents = unionSelections.Where(x => x.AnnouncementId == announcement.Id).OrderBy(x => x.Username).Select(x => new SearchAgentDto
                    {
                        TargetId = x.TargetId,
                        Username = x.Username,
                        IsAgent = x.IsAgent
                    }).ToList();
                }
            }

            //Save lastest announcement being read of client agent
            if (announcements != null && announcements.Any() && query.Type != AnnouncementType.Private.ToInt())
            {
                var existAgentAnnouncement = await agentAnnouncementRepos.FindQuery().FirstOrDefaultAsync(x => x.AgentId == clientAgent.AgentId && x.AnnouncementType == query.Type);
                if (existAgentAnnouncement != null)
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

            return (announcements, new HnMicro.Framework.Responses.ApiResponseMetadata
            {
                NoOfPages = result.Metadata.NoOfPages,
                NoOfRows = result.Metadata.NoOfRows,
                NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                Page = result.Metadata.Page
            });
        }

        public async Task<GetUnreadAnnouncementsResult> GetUnreadAnnouncements(bool isAgent)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var agentAnnouncementRepos = LotteryUow.GetRepository<IAgentAnnouncementRepository>();
            var playerAnnouncementRepos = LotteryUow.GetRepository<IPlayerAnnouncementRepository>();

            var unreadAnnouncements = new List<UnreadAnnouncement>();
            if (isAgent)
            {
                unreadAnnouncements =await GetUnreadAgentAnnouncements(agentRepos, announcementRepos, agentAnnouncementRepos);
            }
            else
            {
                unreadAnnouncements = await GetUnreadPlayerAnnouncements(playerRepos, announcementRepos, playerAnnouncementRepos);
            }

            return new GetUnreadAnnouncementsResult
            {
                UnreadAnnouncements = unreadAnnouncements
            };
        }

        private async Task<List<UnreadAnnouncement>> GetUnreadPlayerAnnouncements(IPlayerRepository playerRepos, IAnnouncementRepository announcementRepos, IPlayerAnnouncementRepository playerAnnouncementRepos)
        {
            var player = await playerRepos.FindByIdAsync(ClientContext.Player.PlayerId) ?? throw new NotFoundException();
            var unreadPlayerAnnouncements = new List<UnreadAnnouncement>();

            foreach (var targetType in Enum.GetValues<AnnouncementType>().Where(x => x != AnnouncementType.Private))
            {
                var lastestPlayerAnnouncement = await playerAnnouncementRepos.FindQuery().FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId && x.AnnouncementType == targetType.ToInt());
                if (lastestPlayerAnnouncement is null) continue;
                unreadPlayerAnnouncements.Add(new UnreadAnnouncement
                {
                    Type = targetType.ToInt(),
                    Quantity = await announcementRepos.FindQueryBy(x => x.Type == targetType.ToInt() && x.Id > lastestPlayerAnnouncement.AnnouncementId).CountAsync()
                });
            }

            return unreadPlayerAnnouncements;
        }

        private async Task<List<UnreadAnnouncement>> GetUnreadAgentAnnouncements(IAgentRepository agentRepos, IAnnouncementRepository announcementRepos, IAgentAnnouncementRepository agentAnnouncementRepos)
        {
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            var unreadAgentAnnouncements = new List<UnreadAnnouncement>();

            foreach (var targetType in Enum.GetValues<AnnouncementType>().Where(x => x != AnnouncementType.Private))
            {
                var lastestAgentAnnouncement = await agentAnnouncementRepos.FindQuery().FirstOrDefaultAsync(x => x.AgentId == clientAgent.AgentId && x.AnnouncementType == targetType.ToInt());
                if (lastestAgentAnnouncement is null) continue;
                unreadAgentAnnouncements.Add(new UnreadAnnouncement
                {
                    Type = targetType.ToInt(),
                    Quantity = await announcementRepos.FindQueryBy(x => x.Type == targetType.ToInt() && x.Id > lastestAgentAnnouncement.AnnouncementId).CountAsync()
                });
            }

            return unreadAgentAnnouncements;
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

        public async Task DeleteMultipleAnnouncement(List<long> selectedIds)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var announcementRepos = LotteryUow.GetRepository<IAnnouncementRepository>();
            var agentAnnouncementRepos = LotteryUow.GetRepository<IAgentAnnouncementRepository>();
            var playerAnnouncementRepos = LotteryUow.GetRepository<IPlayerAnnouncementRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var privateAgentAnnouncementIds = await agentAnnouncementRepos.FindQueryBy(x => selectedIds.Contains(x.AnnouncementId) && x.AnnouncementType == AnnouncementType.Private.ToInt()).Select(x => x.Id).ToListAsync();
            agentAnnouncementRepos.DeleteByIds(privateAgentAnnouncementIds);
            var privatePlayerAnnouncementIds = await playerAnnouncementRepos.FindQueryBy(x => selectedIds.Contains(x.AnnouncementId) && x.AnnouncementType == AnnouncementType.Private.ToInt()).Select(x => x.Id).ToListAsync();
            playerAnnouncementRepos.DeleteByIds(privatePlayerAnnouncementIds);
            
            announcementRepos.DeleteByIds(selectedIds);

            await LotteryUow.SaveChangesAsync();
        }
    }
}
