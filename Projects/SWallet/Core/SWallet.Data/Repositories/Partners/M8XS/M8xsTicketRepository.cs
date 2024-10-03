using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class M8xsTicketRepository : EntityFrameworkCoreRepository<long, M8xsTicket, SWalletContext>, IM8xsTicketRepository
    {
        public M8xsTicketRepository(SWalletContext context) : base(context)
        {
        }
    }
}