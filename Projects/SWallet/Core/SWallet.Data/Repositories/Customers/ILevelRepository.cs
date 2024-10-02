using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public interface ILevelRepository : IEntityFrameworkCoreRepository<int, Level, SWalletContext>
    {
        Task<List<Level>> GetLevelByIds(List<int> levelIds);
    }
}
