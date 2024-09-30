using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Discounts
{
    public class DiscountDetailRepository : EntityFrameworkCoreRepository<long, DiscountDetail, SWalletContext>, IDiscountDetailRepository
    {
        public DiscountDetailRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<List<DiscountDetail>> FindByDiscountId(int discountId)
        {
            return await DbSet.Where(f => f.DiscountId == discountId).ToListAsync();
        }
    }
}
