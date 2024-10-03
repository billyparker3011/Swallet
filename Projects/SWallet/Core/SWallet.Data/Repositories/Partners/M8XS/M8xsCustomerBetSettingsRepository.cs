using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class M8xsCustomerBetSettingsRepository : EntityFrameworkCoreRepository<long, M8xsCustomerBetSetting, SWalletContext>, IM8xsCustomerBetSettingsRepository
    {
        public M8xsCustomerBetSettingsRepository(SWalletContext context) : base(context)
        {
        }
    }
}
