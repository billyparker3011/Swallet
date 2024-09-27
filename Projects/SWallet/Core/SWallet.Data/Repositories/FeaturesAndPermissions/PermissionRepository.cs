using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.FeaturesAndPermissions
{
    public class PermissionRepository : EntityFrameworkCoreRepository<int, Permission, SWalletContext>, IPermissionRepository
    {
        public PermissionRepository(SWalletContext context) : base(context)
        {
        }
    }
}
