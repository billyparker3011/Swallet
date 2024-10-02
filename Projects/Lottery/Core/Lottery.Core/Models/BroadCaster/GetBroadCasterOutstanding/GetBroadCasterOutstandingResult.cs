using Lottery.Core.Dtos.BroadCaster;

namespace Lottery.Core.Models.BroadCaster.GetBroadCasterOutstanding
{
    public class GetBroadCasterOutstandingResult
    {
        public IEnumerable<BroadCasterOutstandingDto> BroadCasterOuts { get; set; }
        public long SummaryBetCount { get; set; }
        public decimal SummaryStake { get; set; }
        public decimal SummaryPayout { get; set; }
    }
}
