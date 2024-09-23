using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Settings
{
    public interface ISettingRepository : IEntityFrameworkCoreRepository<int, Setting, SWalletContext>
    {
        Task<Setting> GetActualSetting();
    }
}
