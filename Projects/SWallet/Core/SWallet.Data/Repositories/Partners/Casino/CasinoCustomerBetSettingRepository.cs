using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public class CasinoCustomerBetSettingRepository : EntityFrameworkCoreRepository<long, CasinoCustomerBetSetting, SWalletContext>, ICasinoCustomerBetSettingRepository
    {
        public CasinoCustomerBetSettingRepository(SWalletContext context) : base(context)
        {
        }
    }
}
