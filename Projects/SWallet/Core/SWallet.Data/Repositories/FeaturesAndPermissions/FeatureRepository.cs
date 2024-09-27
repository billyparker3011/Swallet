using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.FeaturesAndPermissions
{
    public class FeatureRepository : EntityFrameworkCoreRepository<int, Feature, SWalletContext>, IFeatureRepository
    {
        public FeatureRepository(SWalletContext context) : base(context)
        {
        }
    }
}
