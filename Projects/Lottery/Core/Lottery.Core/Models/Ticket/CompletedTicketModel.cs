namespace Lottery.Core.Models.Ticket
{
    public class CompletedTicketModel
    {
        public long TicketId { get; set; }
        public long? ParentId { get; set; }
        public DateTime KickoffTime { get; set; }
        public string ChoosenNumbers { get; set; }
        public decimal Stake { get; set; }
        public int? Prize { get; set; }
        public int? Position { get; set; }
        public string MixedTimes { get; set; }

        public decimal? PlayerOdds { get; set; }
        public decimal PlayerPayout { get; set; }

        public decimal? AgentOdds { get; set; }
        public decimal AgentPayout { get; set; }
        public decimal AgentPt { get; set; }

        public decimal? MasterOdds { get; set; }
        public decimal MasterPayout { get; set; }
        public decimal MasterPt { get; set; }

        public decimal? SupermasterOdds { get; set; }
        public decimal SupermasterPayout { get; set; }
        public decimal SupermasterPt { get; set; }

        public decimal? CompanyOdds { get; set; }
        public decimal CompanyPayout { get; set; }

        public decimal? RewardRate { get; set; }
        public List<CompletedChildrenTicketModel> Children { get; set; }
    }
}
