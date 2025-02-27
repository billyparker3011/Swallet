﻿using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoAgentPositionTakingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoAgentPositionTaking, LotteryContext>
    {
    }
}
