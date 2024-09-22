

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiAgentWinLossModel
    {
        public long AgentId { get; set; }
        public long PlayerId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public long BetCount { get; set; }
        public decimal Payout { get; set; }
        public decimal WinLose { get; set; }
        public BtiWinlossInfoModel AgentWinlose { get; set; }
        public BtiWinlossInfoModel MasterWinlose { get; set; }
        public BtiWinlossInfoModel SupermasterWinlose { get; set; }
        public decimal? Company { get; set; }
    }

    public class BtiWinlossInfoModel
    {
        public decimal WinLose { set; get; }
        public decimal Commission { set; get; }
        public decimal Subtotal => WinLose + Commission;
    }

    public class GetBtiAgentWinLossSummaryResultModel
    {
        public List<BtiAgentWinLossModel> BtiAgentWinlossSummaries { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal TotalWinLose { get; set; }
        public List<TotalBtiAgentWinLoseInfoModel> TotalBtiAgentWinLoseInfo { get; set; }
        public decimal? TotalCompany { get; set; }
    }

    public class TotalBtiAgentWinLoseInfoModel
    {
        public decimal TotalWinLose { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalSubTotal { get; set; }
        public int RoleId { get; set; }
    }
}
