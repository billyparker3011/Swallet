using Lottery.Core.Models.Ticket;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.Tickets
{
    public static class ConvertToTicketDetailModel
    {
        public static TicketDetailModel ToTicketDetailModel(this Ticket ticket)
        {
            return new TicketDetailModel
            {
                TicketId = ticket.TicketId,
                PlayerId = ticket.PlayerId,
                KickoffTime = ticket.KickOffTime,
                BetKindId = ticket.BetKindId,
                ChannelId = ticket.ChannelId,
                RegionId = ticket.RegionId,
                ChoosenNumbers = ticket.ChoosenNumbers,
                State = ticket.State,
                TotalPayout = ticket.PlayerPayout,
                TotalWinlose = ticket.PlayerWinLoss,
                TotalDraftWinlose = ticket.DraftPlayerWinLoss,
                TotalPoints = ticket.Stake,
                CreatedAt = ticket.CreatedAt,
                IpAddress = ticket.IpAddress,
                Platform = ticket.Platform,
                PlayerOdds = ticket.PlayerOdds,
                IsLive = ticket.IsLive,
                MixedTimes = ticket.MixedTimes,
                Position = ticket.Position,
                Prize = ticket.Prize,
                Times = ticket.Times,
                RewardRate = ticket.RewardRate
            };
        }
    }
}
