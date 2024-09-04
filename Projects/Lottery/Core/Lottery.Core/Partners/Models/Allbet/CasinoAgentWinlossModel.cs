
namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentWinlossModel
    {
        public long AgentId { get; set; }
        public long PlayerId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public long BetCount { get; set; }
        public decimal Payout { get; set; }
        public decimal WinLose { get; set; }
        public CasinoWinlossInfoModel AgentWinlose { get; set; }
        public CasinoWinlossInfoModel MasterWinlose { get; set; }
        public CasinoWinlossInfoModel SupermasterWinlose { get; set; }
        public decimal? Company { get; set; }
    }

    public class CasinoWinlossInfoModel
    {
        public decimal WinLose { set; get; }
        public decimal Commission { set; get; }
        public decimal Subtotal => WinLose + Commission;
    }

    public class GetCasinoAgentWinLossSummaryResultModel
    {
        public List<CasinoAgentWinlossModel> CasinoAgentWinlossSummaries { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal TotalWinLose { get; set; }
        public List<TotalCasinoAgentWinLoseInfoModel> TotalCasinoAgentWinLoseInfo { get; set; }
        public decimal? TotalCompany { get; set; }
    }

    public class TotalCasinoAgentWinLoseInfoModel
    {
        public decimal TotalWinLose { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalSubTotal { get; set; }
        public int RoleId { get; set; }
    }
}
