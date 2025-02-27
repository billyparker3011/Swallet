﻿using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightPlayerMappingRepository : IEntityFrameworkCoreRepository<long, CockFightPlayerMapping, LotteryContext>
    {
        Task<CockFightPlayerMapping> FindByMemberRefId(string memberRefId);
    }
}
