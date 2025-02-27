﻿using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;

namespace Lottery.Player.OddsService.Hubs
{
    public interface IOddsHubHandler
    {
        Task CreateConnection(Models.ConnectionInformation connectionInformation);
        void DeleteConnection(string connectionId);
        Task UpdateMatch(UpdateMatchModel model);
        Task UpdateOdds(RateOfOddsValueModel model);
        Task UpdateMixedOdds(MixedRateOfOddsValueModel model);
        Task StartLive(StartLiveEventModel model);
        Task UpdateLiveOdds(UpdateLiveOddsModel model);
    }
}
