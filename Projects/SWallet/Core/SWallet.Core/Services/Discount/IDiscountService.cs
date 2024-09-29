using HnMicro.Core.Scopes;
using SWallet.Core.Models.Discounts;

namespace SWallet.Core.Services.Discount
{
    public interface IDiscountService : IScopedDependency
    {
        Task AddOrUpdateDiscount(AddOrUpdateDiscountModel model);
        Task<GetDiscountsResultModel> GetDiscounts(GetDiscountsModel model);
        Task<List<DiscountModel>> GetStaticDiscount();
    }
}
