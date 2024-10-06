using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoTicketRepository : EntityFrameworkCoreRepository<long, CasinoTicket, SWalletContext>, ICasinoTicketRepository
    {
        public CasinoTicketRepository(SWalletContext context) : base(context)
        {
        }
    }
}
