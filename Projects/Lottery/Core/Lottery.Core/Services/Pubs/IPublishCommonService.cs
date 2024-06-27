using HnMicro.Core.Scopes;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Channel;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;
using Lottery.Core.Models.Prize;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.Services.Pubs
{
    public interface IPublishCommonService : IScopedDependency
    {
        Task PublishBetKind(List<BetKindModel> updatedBetKinds);
        Task PublishChannel(List<ChannelModel> updatedChannels);
        Task PublishPrize(List<PrizeModel> updatedPrizes);
        Task PublishDefaultOdds(List<OddsModel> defaultOdds);
        Task PublishOddsValue(RateOfOddsValueModel rateOfOddsValue);
        Task PublishStartLive(StartLiveEventModel model);
        Task PublishUpdateMatch(UpdateMatchModel model);
        Task PublishUpdateLiveOdds(UpdateLiveOddsModel model);
        Task PublishSetting(SettingModel model);
    }
}
