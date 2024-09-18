using HnMicro.Core.Scopes;
using SWallet.Core.Models.Discounts;

namespace SWallet.Core.Services.Discount
{
    public interface IDiscountService : IScopedDependency
    {
        Task<List<DiscountModel>> GetStaticDiscount();
    }
}
