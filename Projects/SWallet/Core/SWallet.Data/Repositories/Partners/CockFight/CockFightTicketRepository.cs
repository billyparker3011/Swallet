using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class CockFightTicketRepository : EntityFrameworkCoreRepository<long, CockFightTicket, SWalletContext>, ICockFightTicketRepository
    {
        public CockFightTicketRepository(SWalletContext context) : base(context)
        {
        }
    }
}
