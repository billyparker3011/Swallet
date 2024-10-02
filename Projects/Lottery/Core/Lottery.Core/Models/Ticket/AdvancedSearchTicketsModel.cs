using HnMicro.Framework.Models;

namespace Lottery.Core.Models.Ticket
{
    public class AdvancedSearchTicketsModel : QueryAdvance
    {
        public long MatchId { get; set; }
        public List<long> TicketIds { get; set; } = new List<long>();
        public List<string> Username { get; set; } = new List<string>();
        public List<int> BetKindIds { get; set; } = new List<int>();
        public int RegionId { get; set; }
        public int ChannelId { get; set; }
        public List<int> ChooseNumbers { get; set; } = new List<int>();
        public int ContainNumberOperator { get; set; }
        public List<int> States { get; set; } = new List<int>();
        public int? LiveStates { get; set; }
        public List<int> Prizes { get; set; } = new List<int>();
        public List<int> Positions { get; set; } = new List<int>();
    }
}
