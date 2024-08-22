namespace Lottery.Core.Dtos.CockFight
{
    public class CockFightAgentWinlossSummaryDto
    {
        public long AgentId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public long BetCount { get; set; }
        public decimal Payout { get; set; }
        public decimal WinLose { get; set; }
        public CockFightWinlossInfo AgentWinlose { get; set; }
        public CockFightWinlossInfo MasterWinlose { get; set; }
        public CockFightWinlossInfo SupermasterWinlose { get; set; }
        public decimal? Company { get; set; }
    }

    public class CockFightWinlossInfo
    {
        public decimal WinLose { set; get; }
        public decimal Commission { set; get; }
        public decimal Subtotal => WinLose + Commission;
    }
}
