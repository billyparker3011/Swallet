using HnMicro.Modules.EntityFrameworkCore.UnitOfWorks;
using SWallet.Data.Core;

namespace SWallet.Data.UnitOfWorks
{
    public interface ISWalletUow : IEntityFrameworkCoreUnitOfWork<SWalletContext>
    {
    }
}
