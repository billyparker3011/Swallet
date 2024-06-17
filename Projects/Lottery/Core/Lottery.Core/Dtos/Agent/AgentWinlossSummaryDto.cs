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
        public decimal DraftWinLose { get; set; }
        public WinLoseInfo AgentWinlose { get; set; }
        public WinLoseInfo MasterWinlose { get; set; }
        public WinLoseInfo SupermasterWinlose { get; set; }
        public decimal? Company { get; set; }
        public decimal? DraftCompany { get; set; }

        public string IpAddress { get; set; }
        public string Platform { get; set; }
    }

    public class WinLoseInfo
    {
        public decimal WinLose { set; get; }
        public decimal Commission { set; get; }
        public decimal Subtotal { set; get; }

        public decimal DraftWinLose { set; get; }
        public decimal DraftCommission { set; get; }
        public decimal DraftSubtotal { set; get; }
    }
}
