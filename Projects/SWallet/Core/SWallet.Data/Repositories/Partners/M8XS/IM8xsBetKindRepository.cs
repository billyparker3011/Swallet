using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public interface IM8xsBetKindRepository : IEntityFrameworkCoreRepository<int, M8xsBetKind, SWalletContext>
    {
    }
}
