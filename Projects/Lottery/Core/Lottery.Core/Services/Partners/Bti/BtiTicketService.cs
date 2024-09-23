using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Bti;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static Lottery.Core.Partners.Helpers.BtiHelper;

namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiTicketService : LotteryBaseService<BtiTicketService>, IBtiTicketService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IBtiSerivice _btiSerivice;
        private const int _offSetTime = 8;
        private const decimal _testBalance = 20000;

        public BtiTicketService(
            ILogger<BtiTicketService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow,
            IPartnerPublishService partnerPublishService,
            IRedisCacheService redisCacheService,
            IBtiSerivice btiSerivice)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
            _btiSerivice = btiSerivice;
        }

        public async Task<BtiValidateTokenResponseModel> ValidateToken(string token)
        {
            try
            {
                var result = _btiSerivice.ValidateToken(token);

                if (!result.IsValid || result.IsExpired) return new BtiValidateTokenResponseModel(BtiHelper.BtiResponseCodeHelper.Token_Not_Valid);

                var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

                var player = await playerRepos.FindByIdAsync(result.PlayerId);
                var btiPlayerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();
                var btiPlayerBetSettingRepos = LotteryUow.GetRepository<IBtiPlayerBetSettingRepository>();
                var btiAgentBetSettingRepos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
                var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
                var playerMapping = await btiPlayerMappingRepos.FindQueryBy(c => c.PlayerId == result.PlayerId).FirstOrDefaultAsync();
                var playerBetSetting = await btiPlayerBetSettingRepos.FindQueryBy(c => c.PlayerId == result.PlayerId).Include(c => c.BtiBetKind).ToListAsync();

                if (player == null) return new BtiValidateTokenResponseModel(BtiHelper.BtiResponseCodeHelper.Customer_Not_Found);

                if (playerMapping == null)
                {
                    playerMapping = new BtiPlayerMapping()
                    {
                        PlayerId = player.PlayerId,
                        CustomerId = EncodeToBase64(player.PlayerId.ToString()),
                        CustomerLogin = EncodeToBase64(player.Username.ToString()),
                        City = "Beijing",
                        Country = "CN",
                        CurrencyCode = "CNY",
                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        CreatedBy = 0
                    };

                    await btiPlayerMappingRepos.AddAsync(playerMapping);

                    await LotteryUow.SaveChangesAsync();
                }

                var outAndWinlose = await GetOutAndWinlose(btiTicketRepos, playerMapping.PlayerId);

                var balance = _testBalance + outAndWinlose;

                return new BtiValidateTokenResponseModel(BtiHelper.BtiResponseCodeHelper.No_Errors)
                {
                    cust_id = playerMapping.CustomerId,
                    cust_login = playerMapping.CustomerLogin,
                    city = playerMapping.City,
                    country = playerMapping.Country,
                    currency_code = playerMapping.CurrencyCode,
                    balance = balance,
                    data = GetValidationTokenDataModel(playerBetSetting)
                };
            }
            catch (Exception ex)
            {
                return new BtiValidateTokenResponseModel(BtiHelper.BtiResponseCodeHelper.Generic_Error);
            }
        }

        public async Task<BtiDebitReserveResponseModel> Reverse(string cust_id, long reserve_id, decimal amount, string extsessionID, string requestBody)
        {
            var agentRepos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var btiPlayerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();

            var ticket = await btiTicketRepos.FindQueryBy(c => c.CustomerId == cust_id && c.ReserveId == reserve_id && c.Type == BtiTypeHelper.Reverse).FirstOrDefaultAsync();

            if (ticket != null) return new BtiDebitReserveResponseModel(ticket.StatusResponse) { balance = ticket.BalanceResponse, trx_id = ticket.TransactionId.Value };

            var playerMapping = await btiPlayerMappingRepos.FindQueryBy(c=>c.CustomerId == cust_id).Include(c=>c.Player).FirstOrDefaultAsync();

            if(playerMapping == null) return new BtiDebitReserveResponseModel(BtiResponseCodeHelper.Customer_Not_Found) { balance = 0, trx_id = GetTransaction() };

            var outAndWinlose = await GetOutAndWinlose(btiTicketRepos, playerMapping.PlayerId);

            var balance = _testBalance + outAndWinlose - amount;

            if (balance < 0) return new BtiDebitReserveResponseModel(BtiResponseCodeHelper.Insufficient_Funds) { balance = balance + amount, trx_id = GetTransaction() };

            ticket = new Data.Entities.Partners.Bti.BtiTicket
            {
                PlayerId = playerMapping.PlayerId,
                CustomerId = cust_id,
                ReserveId = reserve_id,
                BetAmount = -1 * amount,
                TransactionId = GetTransaction(),
                RequestBody = requestBody,
                BalanceResponse = balance,
                StatusResponse = BtiResponseCodeHelper.No_Errors,
                IsFreeBet = amount == 0,
                Status = BtiTicketStatusHelper.Open,
                Type = BtiTypeHelper.Reverse,
                AgentId = playerMapping.Player.AgentId,
                MasterId = playerMapping.Player.MasterId,
                SupermasterId = playerMapping.Player.SupermasterId,
                BetKindId = 1,
                CurrencyCode = "CNY",
                TicketAmount = -1 * amount,
                TicketCreatedDate = ClockService.GetUtcNow().AddHours(_offSetTime),
                AgentWinLoss = 0,
                AgentPt = 0,
                MasterWinLoss = 0,
                MasterPt = 0,
                SupermasterWinLoss = 0,
                SupermasterPt = 0,
                CompanyWinLoss = 0,
                CreatedAt = ClockService.GetUtcNow().AddHours(_offSetTime),
                CreatedBy = 0
            };

            await btiTicketRepos.AddAsync(ticket);

            await LotteryUow.SaveChangesAsync();

            return new BtiDebitReserveResponseModel(ticket.StatusResponse) { balance = ticket.BalanceResponse, trx_id = ticket.TransactionId.Value };

        }

        public async Task<BtiDebitReserveResponseModel> DebitReverse(string cust_id, long reserve_id, decimal amount, long req_id, long purchase_id, string requestBody)
        {
            var agentRepos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var btiPlayerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();

            var debitTickets = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.Type == BtiTypeHelper.DebitReverse).ToListAsync();

            var debitTicket = debitTickets.FirstOrDefault(c => c.RequestId == req_id && c.PurcahseId == purchase_id);

            if (debitTicket != null) return new BtiDebitReserveResponseModel(debitTicket.StatusResponse) { balance = debitTicket.BalanceResponse, trx_id = debitTicket.TransactionId.Value };

            var reserveTicket = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.Type == BtiTypeHelper.Reverse).FirstOrDefaultAsync();

            var playerMapping = await btiPlayerMappingRepos.FindQueryBy(c => c.CustomerId == cust_id).Include(c => c.Player).FirstOrDefaultAsync() ?? throw new NotFoundException();

            var outAndWinlose = await GetOutAndWinlose(btiTicketRepos, playerMapping.PlayerId);

            var balance = _testBalance + outAndWinlose;

            if (reserveTicket == null) return new BtiDebitReserveResponseModel(BtiResponseCodeHelper.No_Errors) { error_message = "ReserveID Not Exist", balance = balance, trx_id = GetTransaction() };    
            if (reserveTicket.Status == BtiTicketStatusHelper.Cancel) return new BtiDebitReserveResponseModel(BtiResponseCodeHelper.No_Errors) { error_message = "Already cancelled reserve", balance = reserveTicket.BalanceResponse, trx_id = GetTransaction() };

            if (reserveTicket.Status == BtiTicketStatusHelper.Commit) return new BtiDebitReserveResponseModel(BtiResponseCodeHelper.No_Errors) { error_message = "Already committed reserve", balance = reserveTicket.BalanceResponse, trx_id = GetTransaction() };
      
            var ticket = new Data.Entities.Partners.Bti.BtiTicket
            {
                PlayerId = playerMapping.PlayerId,
                CustomerId = cust_id,
                ReserveId = reserve_id,
                RequestId = req_id,
                PurcahseId = purchase_id,
                BetAmount = -1 * amount,
                TransactionId = GetTransaction(),
                RequestBody = requestBody,
                BalanceResponse = reserveTicket.BalanceResponse,
                StatusResponse = BtiResponseCodeHelper.No_Errors,
                IsFreeBet = amount == 0,
                Status = BtiTicketStatusHelper.Debit,
                Type = BtiTypeHelper.DebitReverse,
                ParentId = reserveTicket.Id,
                AgentId = playerMapping.Player.AgentId,
                MasterId = playerMapping.Player.MasterId,
                SupermasterId = playerMapping.Player.SupermasterId,
                BetKindId = 1,
                CurrencyCode = "CNY",
                TicketAmount = reserveTicket.TicketAmount,
                TicketCreatedDate = ClockService.GetUtcNow().AddHours(_offSetTime),
                AgentWinLoss = 0,
                AgentPt = 0,
                MasterWinLoss = 0,
                MasterPt = 0,
                SupermasterWinLoss = 0,
                SupermasterPt = 0,
                CompanyWinLoss = 0,
                CreatedAt = ClockService.GetUtcNow().AddHours(_offSetTime),
                CreatedBy = 0
            };

            await btiTicketRepos.AddAsync(ticket);

            reserveTicket.Status = BtiTicketStatusHelper.Debit;
            reserveTicket.TicketModifiedDate = ClockService.GetUtcNow().AddHours(_offSetTime);

            btiTicketRepos.Update(reserveTicket);

            await LotteryUow.SaveChangesAsync();

            if (-1 * (reserveTicket.BetAmount ?? 0m) - (-1 * debitTickets.Sum(c => c.BetAmount ?? 0m)) - amount  > 0.01m) return new BtiDebitReserveResponseModel(BtiResponseCodeHelper.No_Errors) { error_message = "Total DebitReserve amount larger than Reserve amount ", balance = reserveTicket.BalanceResponse, trx_id = ticket.TransactionId.Value };

            return new BtiDebitReserveResponseModel(ticket.StatusResponse) { balance = ticket.BalanceResponse, trx_id = ticket.TransactionId.Value };

        }

        public async Task<BtiBaseResponseModel> CancelReverse(string cust_id, long reserve_id)
        {
            var agentRepos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var btiPlayerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();

            var cancelTicket = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.Type == BtiTypeHelper.CancelReverse).FirstOrDefaultAsync();

            if (cancelTicket != null) return new BtiBaseResponseModel(cancelTicket.StatusResponse) { balance = cancelTicket.BalanceResponse };

            var debitTickets = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.Type == BtiTypeHelper.DebitReverse).ToListAsync();

            if (debitTickets.Any()) return new BtiBaseResponseModel() { error_message = "Already Debitted Reserve", balance = debitTickets.FirstOrDefault().BalanceResponse };

            var reserveTicket = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.Type == BtiTypeHelper.Reverse).FirstOrDefaultAsync();

            var playerMapping = await btiPlayerMappingRepos.FindQueryBy(c => c.CustomerId == cust_id).Include(c => c.Player).FirstOrDefaultAsync();

            if (playerMapping == null) return new BtiBaseResponseModel(BtiHelper.BtiResponseCodeHelper.Customer_Not_Found);

            var outAndWinlose = await GetOutAndWinlose(btiTicketRepos, playerMapping.PlayerId);

            var balance = _testBalance + outAndWinlose;

            if (reserveTicket == null) return new BtiBaseResponseModel() { error_message = "ReserveID not exists", balance = balance };

            balance = balance + -1 * (reserveTicket.BetAmount ?? 0m);

            var ticket = new Data.Entities.Partners.Bti.BtiTicket
            {
                PlayerId = reserveTicket.PlayerId,
                CustomerId = cust_id,
                ReserveId = reserve_id,
                TransactionId = GetTransaction(),
                BalanceResponse = balance,
                StatusResponse = BtiResponseCodeHelper.No_Errors,
                Status = BtiTicketStatusHelper.Cancel,
                Type = BtiTypeHelper.CancelReverse,
                ParentId = reserveTicket.Id,
                AgentId = reserveTicket.AgentId,
                MasterId = reserveTicket.MasterId,
                SupermasterId = reserveTicket.SupermasterId,
                BetKindId = 1,
                CurrencyCode = "CNY",
                TicketAmount = reserveTicket.TicketAmount,
                TicketCreatedDate = ClockService.GetUtcNow().AddHours(_offSetTime),
                AgentWinLoss = 0,
                AgentPt = 0,
                MasterWinLoss = 0,
                MasterPt = 0,
                SupermasterWinLoss = 0,
                SupermasterPt = 0,
                CompanyWinLoss = 0,
                CreatedAt = ClockService.GetUtcNow().AddHours(_offSetTime),
                CreatedBy = 0
            };

            btiTicketRepos.Add(ticket);

            reserveTicket.Status = BtiTicketStatusHelper.Cancel;
            reserveTicket.TicketModifiedDate = ClockService.GetUtcNow().AddHours(_offSetTime);

            btiTicketRepos.Update(reserveTicket);

            await LotteryUow.SaveChangesAsync();

            return new BtiBaseResponseModel() { balance = balance };
        }

        public async Task<BtiBaseResponseModel> CommitReverse(string cust_id, long reserve_id, long purchase_id)
        {
            var agentRepos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var btiPlayerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();

            var commitTicket = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.PurcahseId == purchase_id && c.Type == BtiTypeHelper.CommitReverse).FirstOrDefaultAsync();

            if(commitTicket != null) return new BtiBaseResponseModel(commitTicket.StatusResponse) { balance = commitTicket.BalanceResponse };

            var reserveTicket = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.Type == BtiTypeHelper.Reverse).FirstOrDefaultAsync();

            var playerMapping = await btiPlayerMappingRepos.FindQueryBy(c => c.CustomerId == cust_id).Include(c => c.Player).FirstOrDefaultAsync();

            if (playerMapping == null) return new BtiBaseResponseModel(BtiHelper.BtiResponseCodeHelper.Customer_Not_Found);

            var outAndWinlose = await GetOutAndWinlose(btiTicketRepos, playerMapping.PlayerId);

            var balance = _testBalance + outAndWinlose;

            if (reserveTicket == null) return new BtiBaseResponseModel() { error_message = "ReserveID not exists", balance = balance };
            
            var debitTickets = await btiTicketRepos.FindQueryBy(c => c.ReserveId == reserve_id && c.Type == BtiTypeHelper.DebitReverse).ToListAsync();

            var betAmountActual = debitTickets.Select(c => c.BetAmount ?? 0m).Sum();

            var difBetAmount = -1 * (reserveTicket.BetAmount ?? 0m) - (-1 * betAmountActual);

            var balanceActual = difBetAmount > 0.01m ? balance + difBetAmount : balance;

            var ticket = new Data.Entities.Partners.Bti.BtiTicket
            {
                PlayerId = reserveTicket.PlayerId,
                CustomerId = cust_id,
                ReserveId = reserve_id,
                PurcahseId = purchase_id,
                TransactionId = GetTransaction(),
                BalanceResponse = balanceActual,
                StatusResponse = BtiResponseCodeHelper.No_Errors,
                Status = BtiTicketStatusHelper.Commit,
                Type = BtiTypeHelper.CommitReverse,
                ParentId = reserveTicket.Id,
                AgentId = reserveTicket.AgentId,
                MasterId = reserveTicket.MasterId,
                SupermasterId = reserveTicket.SupermasterId,
                BetKindId = 1,
                CurrencyCode = "CNY",
                TicketAmount = reserveTicket.TicketAmount,
                TicketCreatedDate = ClockService.GetUtcNow().AddHours(_offSetTime),
                AgentWinLoss = 0,
                AgentPt = 0,
                MasterWinLoss = 0,
                MasterPt = 0,
                SupermasterWinLoss = 0,
                SupermasterPt = 0,
                CompanyWinLoss = 0,
                CreatedAt = ClockService.GetUtcNow().AddHours(_offSetTime),
                CreatedBy = 0
            };

            btiTicketRepos.Add(ticket);

            reserveTicket.Status = BtiTicketStatusHelper.Commit;

            btiTicketRepos.Update(reserveTicket);
            reserveTicket.TicketModifiedDate = ClockService.GetUtcNow().AddHours(_offSetTime);

            foreach (var debitTicket in debitTickets)
            {
                debitTicket.Status = BtiTicketStatusHelper.Commit;
                debitTicket.TicketModifiedDate = ClockService.GetUtcNow().AddHours(_offSetTime);
                btiTicketRepos.Update(debitTicket);
            }

            await LotteryUow.SaveChangesAsync();

            return new BtiBaseResponseModel() { balance = balanceActual, trx_id = ticket.TransactionId.Value };
        }

        public async Task<BtiBaseResponseModel> DebitCustomer(string cust_id, decimal amount, long req_id, long purchase_id, string requestBody)
        {
            var agentRepos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var btiPlayerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();
            var btiAgentPositionTakingRepos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();

            var debitCustomer = await btiTicketRepos.FindQueryBy(c => c.CustomerId == cust_id && c.RequestId == req_id && c.PurcahseId == purchase_id && c.Type == BtiTypeHelper.DebitCustomer).FirstOrDefaultAsync();

            if (debitCustomer != null) return new BtiBaseResponseModel() { balance = debitCustomer.BalanceResponse, trx_id = debitCustomer.TransactionId.Value };

            var playerMapping = await btiPlayerMappingRepos.FindQueryBy(c => c.CustomerId == cust_id).Include(c => c.Player).FirstOrDefaultAsync();

            var outAndWinlose = await GetOutAndWinlose(btiTicketRepos, playerMapping.PlayerId);

            var balance = _testBalance + outAndWinlose - amount;

            var lstAgentId = new List<long> { playerMapping.Player.AgentId, playerMapping.Player.MasterId, playerMapping.Player.SupermasterId };

            var lstPositionTaking = await btiAgentPositionTakingRepos.FindQueryBy(c => lstAgentId.Contains(c.AgentId)).ToListAsync();

            var agentPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.AgentId)?.PositionTaking ?? 0m;
            var masterPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.MasterId)?.PositionTaking ?? 0m;
            var supermasterPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.SupermasterId)?.PositionTaking ?? 0m;

            var ticket = new Data.Entities.Partners.Bti.BtiTicket
            {
                PlayerId = playerMapping.PlayerId,
                CustomerId = cust_id,
                RequestId = req_id,
                PurcahseId = purchase_id,
                TransactionId = GetTransaction(),
                RequestBody = requestBody,
                BalanceResponse = balance,
                StatusResponse = BtiResponseCodeHelper.No_Errors,
                Status = BtiTicketStatusHelper.Complete,
                Type = BtiTypeHelper.DebitCustomer,
                AgentId = playerMapping.Player.AgentId,
                MasterId = playerMapping.Player.MasterId,
                SupermasterId = playerMapping.Player.SupermasterId,
                BetKindId = 1,
                CurrencyCode = "CNY",
                TicketCreatedDate = ClockService.GetUtcNow().AddHours(_offSetTime),
                WinlossAmount = - amount,
                AgentWinLoss = amount * agentPT,
                AgentPt = agentPT,
                MasterWinLoss = (masterPT - agentPT) * amount,
                MasterPt = masterPT,
                SupermasterWinLoss = (supermasterPT - masterPT) * amount,
                SupermasterPt = supermasterPT,
                CompanyWinLoss = (1 - supermasterPT) * amount,
                CreatedAt = ClockService.GetUtcNow().AddHours(_offSetTime),
                CreatedBy = 0
            };

            await btiTicketRepos.AddAsync(ticket);

            await LotteryUow.SaveChangesAsync();

            return new BtiBaseResponseModel() { balance = balance, trx_id = ticket.TransactionId.Value };
        }

        public async Task<BtiBaseResponseModel> CreditCustomer(string cust_id, decimal amount, long req_id, long purchase_id, string requestBody)
        {
            var agentRepos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var btiPlayerMappingRepos = LotteryUow.GetRepository<IBtiPlayerMappingRepository>();
            var btiAgentPositionTakingRepos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();

            var creditCustomer = await btiTicketRepos.FindQueryBy(c => c.CustomerId == cust_id && c.RequestId == req_id && c.PurcahseId == purchase_id && c.Type == BtiTypeHelper.CreditCustomer).FirstOrDefaultAsync();

            if (creditCustomer != null) return new BtiBaseResponseModel() { balance = creditCustomer.BalanceResponse, trx_id = creditCustomer.TransactionId.Value };

            var playerMapping = await btiPlayerMappingRepos.FindQueryBy(c => c.CustomerId == cust_id).Include(c => c.Player).FirstOrDefaultAsync();

            var outAndWinlose = await GetOutAndWinlose(btiTicketRepos, playerMapping.PlayerId);

            var balance = _testBalance + outAndWinlose + amount;

            var lstAgentId = new List<long> { playerMapping.Player.AgentId, playerMapping.Player.MasterId, playerMapping.Player.SupermasterId };

            var lstPositionTaking = await btiAgentPositionTakingRepos.FindQueryBy(c => lstAgentId.Contains(c.AgentId)).ToListAsync();

            var agentPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.AgentId)?.PositionTaking ?? 0m;
            var masterPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.MasterId)?.PositionTaking ?? 0m;
            var supermasterPT = lstPositionTaking.FirstOrDefault(x => x.AgentId == playerMapping.Player.SupermasterId)?.PositionTaking ?? 0m;

            var ticket = new Data.Entities.Partners.Bti.BtiTicket
            {
                PlayerId = playerMapping.PlayerId,
                CustomerId = cust_id,
                RequestId = req_id,
                PurcahseId = purchase_id,
                TransactionId = GetTransaction(),
                RequestBody = requestBody,
                BalanceResponse = balance,
                StatusResponse = BtiResponseCodeHelper.No_Errors,
                Status = BtiTicketStatusHelper.Complete,
                Type = BtiTypeHelper.CreditCustomer,
                AgentId = playerMapping.Player.AgentId,
                MasterId = playerMapping.Player.MasterId,
                SupermasterId = playerMapping.Player.SupermasterId,
                BetKindId = 1,
                CurrencyCode = "CNY",
                TicketCreatedDate = ClockService.GetUtcNow().AddHours(_offSetTime),
                WinlossAmount = amount,
                AgentWinLoss = -1 * amount * agentPT,
                AgentPt = agentPT,
                MasterWinLoss = -1 * (masterPT - agentPT) * amount,
                MasterPt = masterPT,
                SupermasterWinLoss = -1 * (supermasterPT - masterPT) * amount,
                SupermasterPt = supermasterPT,
                CompanyWinLoss = -1 * (1 - supermasterPT) * amount,
                CreatedAt = ClockService.GetUtcNow().AddHours(_offSetTime),
                CreatedBy = 0
            };

            await btiTicketRepos.AddAsync(ticket);

            await LotteryUow.SaveChangesAsync();

            return new BtiBaseResponseModel() { balance = balance, trx_id = ticket.TransactionId.Value };
        }

        private async Task<decimal> GetOutAndWinlose(IBtiTicketRepository btiTicketRepository, long playerId)
        {
            var ticket = await btiTicketRepository.FindQueryBy(c => c.PlayerId == playerId && c.Status != BtiTicketStatusHelper.Cancel && BtiTypeHelper.AmountType.Contains(c.Type)).ToListAsync();

            var commitBetAmount = ticket.Where(c => BtiTypeHelper.DebitReverse == c.Type && BtiTicketStatusHelper.Commit == c.Status).GroupBy(c => c.ReserveId).Select(c =>
            {
                var betActual = -1 * c.Select(x => x.BetAmount ?? 0m).Sum();
                var ticketAmount = -1 * c.FirstOrDefault().TicketAmount ?? 0m;
                if (ticketAmount - betActual > 0.01m) return -1 * betActual;
                else return -1 * ticketAmount;
            }).Sum();

            var reverseBetAmount = ticket.Where(c => BtiTypeHelper.Reverse == c.Type && BtiTicketStatusHelper.Betting.Contains(c.Status)).Select(c => c.BetAmount ?? 0m).Sum();

            var outs = reverseBetAmount + commitBetAmount;
            var winlose = ticket.Select(c => c.WinlossAmount ?? 0m).Sum();
            return outs + winlose;
        }

        private long GetTransaction()
        {
            return DateTime.UtcNow.Ticks;
        }

        private string EncodeToBase64(string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(value);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private bool ValidateSymbols(string value)
        {
            string pattern = @"^[a-zA-Z0-9_]+$";
            return Regex.IsMatch(value, pattern);
        }

        private string GetValidationTokenDataModel(IEnumerable<BtiPlayerBetSetting> btiPlayerBetSettings)
        {
            if (btiPlayerBetSettings.Any())
            {
                var branchs = new List<Branch>();
                var sport = new Sport() { Branches = branchs };
                var data = new ValidationTokenDataModel() { Sport = sport };
                foreach (var btiPlayerBetSetting in btiPlayerBetSettings)
                {
                    if (btiPlayerBetSetting.BtiBetKind.BranchId == 0)
                        data.Sport = new Sport() { MaxBet = btiPlayerBetSetting.MaxBet, MinBet = btiPlayerBetSetting.MinBet };
                    else if (btiPlayerBetSetting.BtiBetKind.BranchId != null)
                        branchs.Add(new Branch { Id = btiPlayerBetSetting.BtiBetKind.BranchId.Value, MaxBet = btiPlayerBetSetting.MaxBet, MinBet = btiPlayerBetSetting.MinBet });
                }

                return ToDataXML(data);
            }
            return null;
        }

        private string ToDataXML(ValidationTokenDataModel model)
        {
            var serializer = new XmlSerializer(typeof(ValidationTokenDataModel));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, model);
                return writer.ToString();
            }
        }
    }
}
