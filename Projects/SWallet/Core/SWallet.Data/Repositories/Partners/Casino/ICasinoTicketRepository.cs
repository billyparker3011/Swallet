using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoTicketRepository : IEntityFrameworkCoreRepository<long, CasinoTicket, SWalletContext>
    {

    }
}
