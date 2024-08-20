using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightTicketRepository : IEntityFrameworkCoreRepository<long, CockFightTicket, LotteryContext>
    {
        Task<CockFightTicket> FindBySId(string sid);
    }
}
