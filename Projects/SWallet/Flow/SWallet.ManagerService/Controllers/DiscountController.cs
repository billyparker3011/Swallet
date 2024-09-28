using HnMicro.Framework.Controllers;
using SWallet.Core.Services.Discount;

namespace SWallet.ManagerService.Controllers
{
    public class DiscountController : HnControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }
    }
}