﻿using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Models.CockFight.GetBalance;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightService : IScopedDependency
    {
        Task CreateCockFightPlayer();
        Task LoginCockFightPlayer();
        Task<LoginPlayerInformationDto> GetCockFightUrl();
        Task<GetCockFightPlayerBalanceResult> GetCockFightPlayerBalance();
    }
}
