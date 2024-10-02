using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Discounts
{
    public interface IDiscountRepository : IEntityFrameworkCoreRepository<int, Discount, SWalletContext>
    {
        Task<List<Discount>> FindStaticDiscounts();
    }
}
