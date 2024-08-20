using HnMicro.Framework.Exceptions;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Player.PlayerService.Requests.CockFight;

namespace Lottery.Player.PlayerService.Helpers.Converters
{
    public static class TicketConvert
    {
        public static Ga28TransferTicketModel ToGa28TransferTicketModel(this TransferTicketRequest request)
        {
            if (request == null || request.Ticket == null) throw new HnMicroException();

            return new Ga28TransferTicketModel
            {
                Id = request.Id,
                Amount = request.Amount,
                Jackpot = request.Jackpot,
                MemberRefId = request.MemberRefId,
                Type = request.Type,
                Ticket = new Ga28RetrieveTicketDataReturnModel
                {
                    AccountId = request.Ticket.AccountId.ToString(),
                    AnteAmount = request.Ticket.AnteAmount,
                    ArenaCode = request.Ticket.ArenaCode,
                    BetAmount = request.Ticket.BetAmount,
                    CreatedDateTime = request.Ticket.CreatedDateTime,
                    CurrencyCode = request.Ticket.CurrencyCode,
                    FightNumber = request.Ticket.FightNumber,
                    IpAddress = request.Ticket.IpAddress,
                    MatchDayCode = request.Ticket.MatchDayCode,
                    MemberRefId = request.Ticket.MemberRefId,
                    ModifiedDateTime = request.Ticket.ModifiedDateTime,
                    Odds = request.Ticket.Odds,
                    OddsType = string.Empty,
                    Result = request.Ticket.Result,
                    Selection = request.Ticket.Selection,
                    SettledDateTime = request.Ticket.SettledDateTime,
                    Sid = request.Ticket.SId,
                    Status = request.Ticket.Status,
                    TicketAmount = request.Ticket.TicketAmount,
                    UserAgent = request.Ticket.UserAgent,
                    ValidStake = string.Empty,
                    WinLossAmount = request.Ticket.WinlossAmount
                }
            };
        }
    }
}
