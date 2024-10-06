using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class CasinoBetKindRepository : EntityFrameworkCoreRepository<int, CasinoBetKind, SWalletContext>, ICasinoBetKindRepository
    {
        public CasinoBetKindRepository(SWalletContext context) : base(context)
        {
        }
    }
}
