namespace Lottery.Core.Dtos.Agent
{
    public class AgentWinlossSummaryDto
    {
        public long AgentId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public long BetCount { get; set; }
        public decimal Point { get; set; }
        public decimal Payout { get; set; }
        public decimal WinLose { get; set; }
        public WinLoseInfo AgentWinlose { get; set; }
        public WinLoseInfo MasterWinlose { get; set; }
        public WinLoseInfo SupermasterWinlose { get; set; }

        public List<WinLoseInfo> WinLoseInfos { get; set; }
        public decimal? Company { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
    }

    public class WinLoseInfo
    {
        public decimal WinLose { set; get; }
        public decimal Commission { set; get; }
        public decimal Subtotal { set; get; }
    }
}
