using Lottery.Core.Models.BetKind;

namespace Lottery.Agent.AgentService.Requests.BetKind
{
    public class ModifyBetKindRequest
    {
        public List<BetKindSettingModel> ModifiedBetKinds { get; set; }
    }
}
