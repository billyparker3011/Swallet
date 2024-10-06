using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories
{
    public interface ICockFightCustomerBetSettingRepository : IEntityFrameworkCoreRepository<long, CockFightCustomerBetSetting, SWalletContext>
    {

    }
}
