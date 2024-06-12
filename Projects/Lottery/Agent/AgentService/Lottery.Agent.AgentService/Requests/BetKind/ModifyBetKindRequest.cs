using Lottery.Core.Models.BetKind;

namespace Lottery.Agent.AgentService.Requests.BetKind
{
    public class ModifyBetKindRequest
    {
        public List<BetKindModel> ModifiedBetKinds { get; set; }
    }
}
