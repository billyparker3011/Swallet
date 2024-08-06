using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAPlayerMappingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAPlayerMapping, LotteryContext>
    {

    }
}
