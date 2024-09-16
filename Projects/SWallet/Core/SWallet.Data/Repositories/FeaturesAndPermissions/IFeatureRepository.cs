using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.FeaturesAndPermissions
{
    public interface IFeatureRepository : IEntityFrameworkCoreRepository<int, Feature, SWalletContext>
    {
    }
}
