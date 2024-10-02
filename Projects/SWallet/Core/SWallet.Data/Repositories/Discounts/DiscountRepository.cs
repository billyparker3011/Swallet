using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Discounts
{
    public class DiscountRepository : EntityFrameworkCoreRepository<int, Discount, SWalletContext>, IDiscountRepository
    {
        public DiscountRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<List<Discount>> FindStaticDiscounts()
        {
            return await DbSet.Where(f => f.IsStatic && f.IsEnabled).ToListAsync();
        }
    }
}
