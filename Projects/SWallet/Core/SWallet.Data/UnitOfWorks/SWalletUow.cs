using HnMicro.Modules.EntityFrameworkCore.UnitOfWorks;
using SWallet.Data.Core;

namespace SWallet.Data.UnitOfWorks
{
    public class SWalletUow : EntityFrameworkCoreUnitOfWork<SWalletContext>, ISWalletUow
    {
        public SWalletUow(SWalletContext context) : base(context)
        {
        }
    }
}
