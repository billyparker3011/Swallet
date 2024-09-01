using HnMicro.Framework.Enums;
using Lottery.Core.Enums;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentOutstandingModel
    {
        public long AgentId { get; set; }
        public string Username { get; set; }
        public Role AgentRole { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalPayout { get; set; }
    }

    public class CasinoAgentOutstandingResultModel
    {
        public long SummaryBetCount { get; set; }
        public decimal SummaryPayout { get; set; }
        public IEnumerable<CasinoAgentOutstandingModel> CasinoAgentOuts { get; set; }
    }

    public class GetCasinoAgentOutstandingModel
    {
        public long? AgentId { get; set; }
        public int? RoleId { get; set; }
        public string SortName { get; set; }
        public SortType SortType { get; set; }
    }
}
