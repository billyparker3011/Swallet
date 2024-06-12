using Lottery.Core.Models.Ticket;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.Tickets
{
    public static class ConvertToTicketDetailModel
    {
        public static TicketDetailModel ToTicketDetailModel(this Ticket f)
        {
            return new TicketDetailModel
            {
                TicketId = f.TicketId,
                KickoffTime = f.KickOffTime,
                BetKindId = f.BetKindId,
                ChannelId = f.ChannelId,
                RegionId = f.RegionId,
                ChoosenNumbers = f.ChoosenNumbers,
                State = f.State,
                TotalPayout = f.PlayerPayout,
                TotalWinlose = f.PlayerWinLoss,
                TotalPoints = f.Stake,
                CreatedAt = f.CreatedAt,
                IpAddress = f.IpAddress,
                Platform = f.Platform,
                PlayerOdds = f.PlayerOdds,
                IsLive = f.IsLive,
                MixedTimes = f.MixedTimes,
                Position = f.Position,
                Prize = f.Prize,
                Times = f.Times,
                RewardRate = f.RewardRate
            };
        }
    }
}
