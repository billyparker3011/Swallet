using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoTicketService : LotteryBaseService<CasinoTicket>, ICasinoTicketService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly ICasinoPlayerMappingService _casinoPlayerMappingService;

        public CasinoTicketService(
           ILogger<CasinoTicket> logger,
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

        public async Task<decimal> ProcessTicketAsync(CasinoTicketModel model, decimal balance)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();

            await CreateCasinoTicketAsync(model);

            var ticketAmout = await repository.FindQueryBy(c => c.BookiePlayerId == model.Player).Select(c=> c.IsCancel? 0 : c.Amount).SumAsync();

            return balance + ticketAmout;

        }

        public async Task<decimal> ProcessCancelTicketAsync(CasinoCancelTicketModel model, decimal balance)
        {
            var repository = LotteryUow.GetRepository<ICasinoTicketRepository>();

            await CreateCasinoCancelTicketAsync(model);

            var ticketAmout = await repository.FindQueryBy(c => c.BookiePlayerId == model.Player).Select(c => c.IsCancel ? 0 : c.Amount).SumAsync();

            return balance + ticketAmout;

        }

        public async Task CreateCasinoTicketAsync(CasinoTicketModel model)
        {
            var playerMapping = await _casinoPlayerMappingService.FindPlayerMappingByBookiePlayerIdAsync(model.Player);

            if (playerMapping == null) throw new NotFoundException();

            var casinoTicketRepository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var casinoTicketEventDetailRepository = LotteryUow.GetRepository<ICasinoTicketEventDetailRepository>();

            var casinoTicket = new CasinoTicket()
            {
                TransactionId = model.TranId,
                PlayerId = playerMapping.PlayerId,
                BookiePlayerId = model.Player,
                Amount = model.Amount,
                Currency = model.Currency,
                Reason = model.Reason,
                Type = model.Type,
                IsRetry = model.IsRetry,
                CreatedAt = DateTime.Now,
                CreatedBy = 0

            };

            await casinoTicketRepository.AddAsync(casinoTicket);

            if (CasinoHelper.TypeTransfer.BetDetails.Contains(casinoTicket.Type) && model.CasinoTicketBetDetailModels.Any())
            {

                var casinoTicketBetDetails = new List<CasinoTicketBetDetail>();
                casinoTicketBetDetails.AddRange(model.CasinoTicketBetDetailModels.Select(c =>
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

                      CreatedAt = DateTime.Now,
                      CreatedBy = 0,
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

                      CreatedAt = DateTime.Now,
                      CreatedBy = 0,
                  }).ToList());

                await casinoTicketEventDetailRepository.AddRangeAsync(casinoTicketEventDetails);

            }

            await LotteryUow.SaveChangesAsync();
        }

        public async Task CreateCasinoCancelTicketAsync(CasinoCancelTicketModel model)
        {
            var casinoTicketRepository = LotteryUow.GetRepository<ICasinoTicketRepository>();

            var ticket = await casinoTicketRepository.FindQueryBy(c => c.TransactionId == model.OriginalTranId).FirstOrDefaultAsync();

            if (ticket == null) throw new NotFoundException();

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

                  CreatedAt = DateTime.Now,
                  CreatedBy = 0,
              }).ToList());

            await casinoTicketBetDetailRepository.AddRangeAsync(casinoTicketBetDetails);

            await LotteryUow.SaveChangesAsync();
        }
    }
}
