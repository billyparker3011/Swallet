using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using static Lottery.Core.Helpers.PartnerHelper;
using static Lottery.Core.Partners.Helpers.CasinoHelper;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoTicketService : LotteryBaseService<CasinoTicketService>, ICasinoTicketService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly ICasinoPlayerMappingService _casinoPlayerMappingService;

        public CasinoTicketService(
           ILogger<CasinoTicketService> logger,
           IServiceProvider serviceProvider,
           IConfiguration configuration,
           IClockService clockService,
           ILotteryClientContext clientContext,
           ILotteryUow lotteryUow,
           IRedisCacheService redisCacheService,
           ICasinoPlayerMappingService casinoPlayerMappingService)
           : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _redisCacheService = redisCacheService;
            _casinoPlayerMappingService = casinoPlayerMappingService;
        }

        public async Task<CasinoTicket> FindAsync(long id)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            return await repository.FindByIdAsync(id);
        }

        public async Task<CasinoTicket> FindWithIncludeAsync(long id)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            return await repository.FindQueryBy(c => c.Id == id).Include(c => c.Player).Include(c => c.CasinoTicketBetDetails).Include(c => c.CasinoTicketEventDetails).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CasinoTicket>> GetsByPlayerIdAsync(long playerId)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            return await repository.FindByAsync(c => c.PlayerId == playerId);
        }

        public async Task<IEnumerable<CasinoTicket>> GetsByPlayerIdWithIncludeAsync(long playerId)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            return await repository.FindQueryBy(c => c.PlayerId == playerId).Include(c => c.Player).Include(c => c.CasinoTicketBetDetails).Include(c => c.CasinoTicketEventDetails).ToListAsync();
        }

        public async Task<IEnumerable<CasinoTicket>> GetsByBookiePlayerIdAsync(string bookiePlayerId)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            return await repository.FindByAsync(c => c.BookiePlayerId == bookiePlayerId);
        }

        public async Task<IEnumerable<CasinoTicket>> GetsByBookiePlayerIdWithIncludeAsync(string bookiePlayerId)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            return await repository.FindQueryBy(c => c.BookiePlayerId == bookiePlayerId).Include(c => c.Player).Include(c => c.CasinoTicketBetDetails).Include(c => c.CasinoTicketEventDetails).ToListAsync();
        }

        public async Task<IEnumerable<CasinoTicket>> GetAllAsync()
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            return repository.GetAll();
        }

        public async Task<decimal> GetBalanceAsync(string player, decimal balance)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();

            var ticketAmout = await repository.FindQueryBy(c => c.BookiePlayerId == player && c.Type != CasinoHelper.TypeTransfer.ManualSettle).Select(c => c.IsCancel ? 0 : c.Amount).SumAsync();

            return balance + ticketAmout;

        }

        public async Task<(decimal, int)> ProcessTicketAsync(CasinoTicketModel model, decimal balance)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();

            var ticketAmout = await repository.FindQueryBy(c => c.BookiePlayerId == model.Player && c.Type != CasinoHelper.TypeTransfer.ManualSettle).Select(c => c.IsCancel ? 0 : c.Amount).SumAsync();

            if ((balance + ticketAmout + model.Amount) < 0) return (-1, CasinoReponseCode.Credit_is_not_enough);

            var code = await CreateCasinoTicketAsync(model);

            if (model.Type == CasinoHelper.TypeTransfer.ManualSettle) return (balance + ticketAmout, CasinoReponseCode.Success);

            return (balance + ticketAmout + model.Amount, code);

        }

        public async Task<(decimal, int)> ProcessCancelTicketAsync(CasinoCancelTicketModel model, decimal balance)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();

            var code = await CreateCasinoCancelTicketAsync(model);

            var ticketAmout = await repository.FindQueryBy(c => c.BookiePlayerId == model.Player && c.Type != CasinoHelper.TypeTransfer.ManualSettle).Select(c => c.IsCancel ? 0 : c.Amount).SumAsync();

            return (balance + ticketAmout, code);

        }

        public async Task<int> CreateCasinoTicketAsync(CasinoTicketModel model)
        {
            var casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            var playerMapping = await casinoPlayerMappingRepository.FindQueryBy(c => c.BookiePlayerId == model.Player).Include(c => c.Player).FirstOrDefaultAsync();

            if (playerMapping == null) return CasinoReponseCode.Player_account_does_not_exist;

            var casinoTicketRepository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            var casinoBetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var casinoTicketEventDetailRepository = LotteryUow.GetRepository<ICasinoTicketEventDetailRepository>();
            var casinoAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();

            var casinoTicket = await casinoTicketRepository.FindQueryBy(c => c.TransactionId == model.TranId).FirstOrDefaultAsync();

            if (casinoTicket != null) return CasinoReponseCode.Invalid_status;

            var lstAgentId = new List<long> { playerMapping.Player.AgentId, playerMapping.Player.MasterId, playerMapping.Player.SupermasterId };

            var lstPositionTaking = await casinoAgentPositionTakingRepository.FindQueryBy(c => lstAgentId.Contains(c.AgentId)).ToListAsync();

            var winOrLoseAmountTotal = model.CasinoTicketBetDetailModels.Select(c => c.WinOrLossAmount - (model.Type == CasinoTransferType.Manual_Settle ? c.BetAmount : 0m) ?? 0m).Sum();

            var agentPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.AgentId)?.PositionTaking ?? 0m;
            var masterPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.MasterId)?.PositionTaking ?? 0m;
            var supermasterPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.SupermasterId)?.PositionTaking ?? 0m;

            var betKind = await casinoBetKindRepository.FindQuery().FirstOrDefaultAsync();

            casinoTicket = new CasinoTicket()
            {
                TransactionId = model.TranId,
                PlayerId = playerMapping.PlayerId,
                BookiePlayerId = model.Player,
                Amount = model.Amount,
                Currency = model.Currency,
                Reason = model.Reason,
                Type = model.Type,
                IsRetry = model.IsRetry,

                AgentId = playerMapping.Player.AgentId,
                BetKindId = betKind != null ? betKind.Id : 0,
                MasterId = playerMapping.Player.MasterId,
                SupermasterId = playerMapping.Player.SupermasterId,

                AgentPt = agentPT,
                AgentWinLoss = -1 * winOrLoseAmountTotal * agentPT,
                MasterPt = masterPT,
                MasterWinLoss = -1 * (masterPT - agentPT) * winOrLoseAmountTotal,
                SupermasterPt = supermasterPT,
                SupermasterWinLoss = -1 * (supermasterPT - masterPT) * winOrLoseAmountTotal,
                CompanyWinLoss = -1 * (1 - supermasterPT) * winOrLoseAmountTotal,

                WinlossAmountTotal = winOrLoseAmountTotal,

                CreatedAt = DateTime.UtcNow.AddHours(8),
                CreatedBy = 0

            };

            await casinoTicketRepository.AddAsync(casinoTicket);

            if (CasinoHelper.TypeTransfer.BetDetails.Contains(casinoTicket.Type) && model.CasinoTicketBetDetailModels.Any())
            {

                var casinoTicketBetDetails = new List<CasinoTicketBetDetail>();
                casinoTicketBetDetails.AddRange(model.CasinoTicketBetDetailModels.Select(c =>
                {
                    var winOrLossAmountAct = (c.WinOrLossAmount ?? 0m) - (casinoTicket.Type == TypeTransfer.ManualSettle ? c.BetAmount : 0m);
                    return
                    new CasinoTicketBetDetail()
                    {
                        BetNum = c.BetNum,
                        GameRoundId = c.GameRoundId,
                        Status = c.Status,
                        BetAmount = c.BetAmount,
                        Deposit = c.Deposit,
                        GameType = c.GameType,
                        BetType = c.BetType,
                        Commission = c.Commission,
                        ExchangeRate = c.ExchangeRate,
                        GameResult = c.GameResult,
                        GameResult2 = c.GameResult2,
                        WinOrLossAmount = c.WinOrLossAmount,
                        ValidAmount = c.ValidAmount,
                        BetTime = c.BetTime,
                        TableName = c.TableName,
                        BetMethod = c.BetMethod,
                        AppType = c.AppType,
                        GameRoundEndTime = c.GameRoundEndTime,
                        GameRoundStartTime = c.GameRoundStartTime,
                        Ip = c.Ip,
                        CasinoTicket = casinoTicket,

                        AgentWinLoss = -1 * winOrLossAmountAct * agentPT,
                        MasterWinLoss = -1 * (masterPT - agentPT) * winOrLossAmountAct,
                        SupermasterWinLoss = -1 * (supermasterPT - masterPT) * winOrLossAmountAct,
                        CompanyWinLoss = -1 * (1 - supermasterPT) * winOrLossAmountAct,

                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        CreatedBy = 0,
                    };
                }).ToList());

                await casinoTicketBetDetailRepository.AddRangeAsync(casinoTicketBetDetails);
            }

            if (CasinoHelper.TypeTransfer.EventDetails.Contains(casinoTicket.Type) && model.CasinoTicketEventDetailModels.Any())
            {

                var casinoTicketEventDetails = new List<CasinoTicketEventDetail>();
                casinoTicketEventDetails.AddRange(model.CasinoTicketEventDetailModels.Select(c =>
                  new CasinoTicketEventDetail()
                  {
                      EventType = c.EventType,
                      EventCode = c.EventCode,
                      EventRecordNum = c.EventRecordNum,
                      Amount = c.Amount,
                      ExchangeRate = c.ExchangeRate,
                      SettleTime = c.SettleTime,
                      CasinoTicket = casinoTicket,

                      CreatedAt = DateTime.UtcNow.AddHours(8),
                      CreatedBy = 0,
                  }).ToList());

                await casinoTicketEventDetailRepository.AddRangeAsync(casinoTicketEventDetails);

            }

            await LotteryUow.SaveChangesAsync();
            return CasinoReponseCode.Success;
        }

        public async Task<int> CreateCasinoCancelTicketAsync(CasinoCancelTicketModel model)
        {
            var casinoTicketRepository = LotteryUow.GetRepository<ICasinoTicketRepository>();

            var ticket = await casinoTicketRepository.FindQueryBy(c => c.TransactionId == model.OriginalTranId).FirstOrDefaultAsync();

            if (ticket == null) return CasinoReponseCode.Transaction_not_existed;

            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();

            ticket.IsCancel = true;
            ticket.CancelReason = model.Reason;
            ticket.CancelTransactionId = model.TranId;
            ticket.IsRetryCancel = model.IsRetry;
            ticket.UpdatedAt = DateTime.Now;

            casinoTicketRepository.Update(ticket);

            var casinoTicketBetDetails = new List<CasinoTicketBetDetail>();
            casinoTicketBetDetails.AddRange(model.OriginalDetails.Select(c =>
              new CasinoTicketBetDetail()
              {
                  BetNum = c.BetNum,
                  GameRoundId = c.GameRoundId,
                  Status = c.Status,
                  BetAmount = c.BetAmount,
                  Deposit = c.Deposit,
                  GameType = c.GameType,
                  BetType = c.BetType,
                  Commission = c.Commission,
                  ExchangeRate = c.ExchangeRate,
                  GameResult = c.GameResult,
                  GameResult2 = c.GameResult2,
                  WinOrLossAmount = c.WinOrLossAmount,
                  ValidAmount = c.ValidAmount,
                  BetTime = c.BetTime,
                  TableName = c.TableName,
                  BetMethod = c.BetMethod,
                  AppType = c.AppType,
                  GameRoundEndTime = c.GameRoundEndTime,
                  GameRoundStartTime = c.GameRoundStartTime,
                  Ip = c.Ip,
                  IsCancel = true,
                  CasinoTicket = ticket,

                  CreatedAt = DateTime.UtcNow.AddHours(8),
                  CreatedBy = 0,
              }).ToList());

            await casinoTicketBetDetailRepository.AddRangeAsync(casinoTicketBetDetails);

            await LotteryUow.SaveChangesAsync();

            return CasinoReponseCode.Success;
        }

        public async Task<List<CasinoBetTableTicketModel>> GetCasinoTicketsAsBetList()
        {
            var casinoBetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var casinoGameTypeRepository = LotteryUow.GetRepository<ICasinoGameTypeRepository>();

            var betRunnings = CasinoBetStatus.BetRunning.ToList();
            var betCompleted = CasinoBetStatus.BetCompleted.ToList();

            var casinoTicketBetDetails = await casinoTicketBetDetailRepository.FindQueryBy(c => c.CasinoTicket.PlayerId == ClientContext.Player.PlayerId && !c.IsCancel).Include(c => c.CasinoTicket).OrderByDescending(c => c.Id).ToListAsync();

            var betEnums = casinoTicketBetDetails.Select(c => c.GameRoundId).Distinct().ToList();
            var result = new List<CasinoBetTableTicketModel>();
            betEnums.ForEach(c =>
            {
                var details = casinoTicketBetDetails.Where(x => x.GameRoundId == c).ToList();
                var casinoTicketBetDetail = details.FirstOrDefault();
                var betKind = casinoBetKindRepository.FindById(casinoTicketBetDetail.CasinoTicket.BetKindId);
                var casinoTicketBetDetailModels = details.Select(x => ToBetTableTicketDetailModel(x)).ToList();
                result.Add(new CasinoBetTableTicketModel()
                {

                    GameRoundId = c,
                    DateCreated = details.Min(c => c.CreatedAt),
                    TableName = casinoTicketBetDetailModels.FirstOrDefault().GameTypeName + " " + casinoTicketBetDetail.TableName,
                    BetTypeName = casinoTicketBetDetailModels.FirstOrDefault().BetTypeName,
                    BetAmount = casinoTicketBetDetailModels.Select(c => (c.Status == CasinoTransferType.Settle && c.Type != CasinoTransferType.Manual_Settle) ? 0m : c.BetAmount).Sum(),
                    WinlossAmountTotal = details.Select(c => c.CasinoTicket.WinlossAmountTotal).Sum(),
                    Ip = casinoTicketBetDetail.Ip,

                    BetKindId = casinoTicketBetDetail.CasinoTicket.BetKindId,
                    BetKindName = betKind?.Name,
                    BookiePlayerId = casinoTicketBetDetail.CasinoTicket.BookiePlayerId,
                    BetNum = casinoTicketBetDetail.BetNum,
                    Currency = casinoTicketBetDetail.CasinoTicket.Currency,
                    PlayerId = casinoTicketBetDetail.CasinoTicket.PlayerId,
                    CasinoTicketBetDetailModels = casinoTicketBetDetailModels,

                });

            });

            return result.ToList();
        }

        public async Task<List<CasinoBetTableTicketModel>> GetCasinoRefundRejectTickets()
        {
            var casinoBetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();

            var query = casinoTicketBetDetailRepository.FindQueryBy(c => c.CasinoTicket.PlayerId == ClientContext.Player.PlayerId).Include(c => c.CasinoTicket);
            var queryTicketRefund = query.Where(c => CasinoBetStatus.Refund == c.Status).Select(c => c.GameRoundId).Distinct();
            var casinoTicketBetDetails = await query.Where(c => queryTicketRefund.Contains(c.GameRoundId) && !c.IsCancel).OrderByDescending(c => c.Id).ToListAsync();

            var betEnums = casinoTicketBetDetails.Select(c => c.GameRoundId).Distinct().ToList();
            var result = new List<CasinoBetTableTicketModel>();
            betEnums.ForEach(c =>
            {
                var details = casinoTicketBetDetails.Where(x => x.GameRoundId == c).ToList();
                var casinoTicketBetDetail = details.FirstOrDefault();
                var betKind = casinoBetKindRepository.FindById(casinoTicketBetDetail.CasinoTicket.BetKindId);
                var casinoTicketBetDetailModels = details.Select(x => ToBetTableTicketDetailModel(x)).ToList();
                result.Add(new CasinoBetTableTicketModel()
                {

                    GameRoundId = c,
                    DateCreated = details.Min(c => c.CreatedAt),
                    TableName = casinoTicketBetDetailModels.FirstOrDefault().GameTypeName + " " + casinoTicketBetDetail.TableName,
                    BetTypeName = casinoTicketBetDetailModels.FirstOrDefault().BetTypeName,
                    BetAmount = casinoTicketBetDetailModels.Select(c => (c.Status == CasinoTransferType.Settle && c.Type != CasinoTransferType.Manual_Settle) ? 0m : c.BetAmount).Sum(),
                    WinlossAmountTotal = details.Select(c => c.CasinoTicket.WinlossAmountTotal).Sum(),
                    Ip = casinoTicketBetDetail.Ip,

                    BetKindId = casinoTicketBetDetail.CasinoTicket.BetKindId,
                    BetKindName = betKind?.Name,
                    BookiePlayerId = casinoTicketBetDetail.CasinoTicket.BookiePlayerId,
                    BetNum = casinoTicketBetDetail.BetNum,
                    Currency = casinoTicketBetDetail.CasinoTicket.Currency,
                    PlayerId = casinoTicketBetDetail.CasinoTicket.PlayerId,
                    CasinoTicketBetDetailModels = casinoTicketBetDetailModels,

                });
            });

            return result;
        }

        public CasinoBetTableTicketDetailModel ToBetTableTicketDetailModel(CasinoTicketBetDetail item)
        {
            if (item == null) return new CasinoBetTableTicketDetailModel();
            return new CasinoBetTableTicketDetailModel()
            {
                BetNum = item.BetNum,
                GameRoundId = item.GameRoundId,
                Status = item.Status,
                StatusName = FindStaticFieldName(typeof(CasinoBetStatus), item.Status),
                BetAmount = item.BetAmount,
                Deposit = item.Deposit,
                GameType = item.GameType,
                GameTypeName = FindStaticFieldName(typeof(PartnerHelper.CasinoGameType), item.GameType),
                BetType = item.BetType,
                BetTypeName = FindStaticFieldName(typeof(CasinoBetType), item.BetType),
                Commission = item.Commission,
                ExchangeRate = item.ExchangeRate,
                GameResult = item.GameResult,
                GameResult2 = item.GameResult2,
                WinOrLossAmount = item.WinOrLossAmount,
                ValidAmount = item.ValidAmount,
                BetTime = item.BetTime,
                TableName = item.TableName,
                BetMethod = item.BetMethod,
                BetMethodName = FindStaticFieldName(typeof(CasinoBetMethod), (int)item.BetMethod),
                AppType = item.AppType,
                AppTypeName = FindStaticFieldName(typeof(PartnerHelper.CasinoAppType), (int)item.AppType),
                GameRoundStartTime = item.GameRoundStartTime,
                GameRoundEndTime = item.GameRoundEndTime,
                Type = item.CasinoTicket?.Type ?? 0,
                TypeName = FindStaticFieldName(typeof(PartnerHelper.CasinoTransferType), item.CasinoTicket?.Type ?? 0),
                Ip = item.Ip
            };
        }
    }
}
