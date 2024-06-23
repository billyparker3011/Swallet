using Lottery.Core.Models.MatchResult;

namespace Lottery.Match.MatchService.Requests.MatchResult
{
    public class UpdateResultRequest
    {
        public List<PrizeResultModel> Results { get; set; }
    }
}
