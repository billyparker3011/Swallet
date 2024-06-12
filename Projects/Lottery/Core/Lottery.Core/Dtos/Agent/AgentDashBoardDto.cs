namespace Lottery.Core.Dtos.Agent
{
    public class AgentDashBoardDto
    {
        public BalanceInfo Balance { get; set; }
        public StatisticsInfo Statistics { get; set; }
    }
    public class BalanceInfo
    {
        public string Username { get; set; }
        public string UserRole { get; set; }
        public decimal? Currency { get; set; }
        public long Point { get; set; }
        public decimal Cash { get; set; }
        public long YesterdayPoint { get; set; }
        public decimal YesterdayCash { get; set; }
        public decimal TodayWinLoss { get; set; }
        public DateTime Today { get; set; }
        public decimal YesterdayWinLoss { get; set; }
        public DateTime Yesterday { get; set; }
        public decimal? AgentGiven { get; set; }
        public decimal TotalGivenOfLowerGrade { get; set; }
    }

    public class StatisticsInfo
    {
        public decimal TotalOutstanding { get; set; }
        public List<AgenStateInfo> AgenStateInfos { get; set; }
        public int TotalNewPlayerOfAMonth { get; set; }
    }

    public class AgenStateInfo
    {
        public string AgentName { get; set; }
        public int TotalAgentOpen { get; set; }
        public int TotalAgentSuspended { get; set; }
        public int TotalAgentClosed { get; set; }
    }

    public class AgentSumarryInfo
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Credit { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
