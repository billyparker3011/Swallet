using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Discounts
{
    public interface IDiscountDetailRepository : IEntityFrameworkCoreRepository<long, DiscountDetail, SWalletContext>
    {
        Task<List<DiscountDetail>> FindByDiscountId(int discountId);
        Task<DiscountDetail> FindByReferenceTransaction(Guid? referenceDiscountDetail);
    }
}
