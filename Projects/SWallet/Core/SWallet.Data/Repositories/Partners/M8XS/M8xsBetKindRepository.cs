using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class M8xsBetKindRepository : EntityFrameworkCoreRepository<int, M8xsBetKind, SWalletContext>, IM8xsBetKindRepository
    {
        public M8xsBetKindRepository(SWalletContext context) : base(context)
        {
        }
    }
}
