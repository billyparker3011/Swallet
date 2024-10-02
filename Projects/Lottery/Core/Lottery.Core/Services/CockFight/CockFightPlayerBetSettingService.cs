using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetSetting;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightPlayerBetSettingService : LotteryBaseService<CockFightPlayerBetSettingService>, ICockFightPlayerBetSettingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;
        public CockFightPlayerBetSettingService(
            ILogger<CockFightPlayerBetSettingService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow,
            IPartnerPublishService partnerPublishService,
            IRedisCacheService redisCacheService,
            IAuditService auditService)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
            _auditService = auditService;
        }

        public async Task<GetCockFightAgentBetSettingResult> GetCockFightPlayerBetSettingDetail(long playerId)
        {
            //Init repos
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightPlayerOddRepos = LotteryUow.GetRepository<ICockFightPlayerBetSettingRepository>();
            var cockFightAgentOddRepos = LotteryUow.GetRepository<ICockFightAgentBetSettingRepository>();

            var targetPlayer = await playerRepos.FindByIdAsync(playerId) ?? throw new NotFoundException();

            var defaultBetSetting = await cockFightAgentOddRepos.FindQuery().Include(x => x.Agent).FirstOrDefaultAsync(x => x.Agent.RoleId == Role.Agent.ToInt() && x.AgentId == targetPlayer.AgentId);

            return await cockFightPlayerOddRepos.FindQuery()
                                                .Include(x => x.Player)
                                                .Include(x => x.CockFightBetKind)
                                                .Where(x => x.Player.PlayerId == targetPlayer.PlayerId)
                                                .Select(x => new GetCockFightAgentBetSettingResult
                                                {
                                                    BetKindId = x.CockFightBetKind.Id,
                                                    BetKindName = x.CockFightBetKind.Name,
                                                    MainLimitAmountPerFight = x.MainLimitAmountPerFight,
                                                    DefaultMaxMainLimitAmountPerFight = defaultBetSetting != null ? defaultBetSetting.MainLimitAmountPerFight : 0m,
                                                    DrawLimitAmountPerFight = x.DrawLimitAmountPerFight,
                                                    DefaultMaxDrawLimitAmountPerFight = defaultBetSetting != null ? defaultBetSetting.DrawLimitAmountPerFight : 0m,
                                                    LimitNumTicketPerFight = x.LimitNumTicketPerFight,
                                                    DefaultMaxLimitNumTicketPerFight = defaultBetSetting != null ? defaultBetSetting.LimitNumTicketPerFight : 0m
                                                })
                                                .FirstOrDefaultAsync();
        }

        public async Task UpdateCockFightPlayerBetSetting(long playerId, UpdateCockFightAgentBetSettingModel model)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightBetKindRepos = LotteryUow.GetRepository<ICockFightBetKindRepository>();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            var player = await playerRepository.FindByIdAsync(playerId) ?? throw new NotFoundException();

            var cockFightPlayerOddRepository = LotteryUow.GetRepository<ICockFightPlayerBetSettingRepository>();

            var auditBetSettings = new List<AuditSettingData>();
            var updatedCockFightBetKind = await cockFightBetKindRepos.FindByIdAsync(model.BetKindId) ?? throw new NotFoundException();
            var existedCfPlayerBetSetting = await cockFightPlayerOddRepository.FindQueryBy(x => x.PlayerId == player.PlayerId && x.BetKindId == updatedCockFightBetKind.Id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var cockFightPlayerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.PlayerId == player.PlayerId).FirstOrDefaultAsync();

            // Update target cockfight agent bet setting
            var oldMainLimitAmountPerFight = existedCfPlayerBetSetting.MainLimitAmountPerFight;
            var oldDrawLimitAmountPerFight = existedCfPlayerBetSetting.DrawLimitAmountPerFight;
            var oldLimitNumTicketPerFight = existedCfPlayerBetSetting.LimitNumTicketPerFight;
            existedCfPlayerBetSetting.MainLimitAmountPerFight = model.MainLimitAmountPerFight;
            existedCfPlayerBetSetting.DrawLimitAmountPerFight = model.DrawLimitAmountPerFight;
            existedCfPlayerBetSetting.LimitNumTicketPerFight = model.LimitNumTicketPerFight;

            if (oldMainLimitAmountPerFight != existedCfPlayerBetSetting.MainLimitAmountPerFight || oldDrawLimitAmountPerFight != existedCfPlayerBetSetting.DrawLimitAmountPerFight || oldLimitNumTicketPerFight != existedCfPlayerBetSetting.LimitNumTicketPerFight)
            {
                auditBetSettings.AddRange(new List<AuditSettingData>
                    {
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightMainLimitAmountPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldMainLimitAmountPerFight,
                            NewValue = existedCfPlayerBetSetting.MainLimitAmountPerFight
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightDrawLimitAmountPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldDrawLimitAmountPerFight,
                            NewValue = existedCfPlayerBetSetting.DrawLimitAmountPerFight
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightLimitNumTicketPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldLimitNumTicketPerFight,
                            NewValue = existedCfPlayerBetSetting.LimitNumTicketPerFight
                        }
                    });
            }
            await LotteryUow.SaveChangesAsync();

            if (auditBetSettings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = AuditType.Setting.ToInt(),
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = player.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdateBetSetting,
                    SupermasterId = player.SupermasterId,
                    MasterId = player.MasterId,
                    AgentId = player.AgentId,
                    AuditSettingDatas = auditBetSettings.OrderBy(x => x.BetKind).ToList()
                });
            }

            // Sync bet setting to Ga28
            if(cockFightPlayerMapping != null)
            {
                try
                {
                    await _partnerPublishService.Publish(new Ga28SyncUpBetSettingModel
                    {
                        Partner = PartnerType.GA28,
                        MainLimitAmountPerFight = existedCfPlayerBetSetting.MainLimitAmountPerFight,
                        DrawLimitAmountPerFight = existedCfPlayerBetSetting.DrawLimitAmountPerFight,
                        LimitNumTicketPerFight = existedCfPlayerBetSetting.LimitNumTicketPerFight,
                        AccountId = cockFightPlayerMapping.AccountId,
                        MemberRefId = cockFightPlayerMapping.MemberRefId
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Update bet setting cock fight player with id = {existedCfPlayerBetSetting.PlayerId} failed. Detail : {ex}");
                }
            }
        }
    }
}
