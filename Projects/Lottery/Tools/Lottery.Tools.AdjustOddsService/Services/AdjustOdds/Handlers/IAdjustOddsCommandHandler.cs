﻿using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Handlers
{
    public interface IAdjustOddsCommandHandler
    {
        string Command { get; set; }

        void Handler(AdjustOddsCommand command);
    }
}
