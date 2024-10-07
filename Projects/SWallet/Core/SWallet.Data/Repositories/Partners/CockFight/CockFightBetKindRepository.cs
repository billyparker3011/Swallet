using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class CockFightBetKindRepository : EntityFrameworkCoreRepository<int, CockFightBetKind, SWalletContext>, ICockFightBetKindRepository
    {
        public CockFightBetKindRepository(SWalletContext context) : base(context)
        {
        }
    }
}
