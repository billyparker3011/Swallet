using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class CockFightCustomerBetSettingRepository : EntityFrameworkCoreRepository<long, CockFightCustomerBetSetting, SWalletContext>, ICockFightCustomerBetSettingRepository
    {
        public CockFightCustomerBetSettingRepository(SWalletContext context) : base(context)
        {
        }
    }
}
