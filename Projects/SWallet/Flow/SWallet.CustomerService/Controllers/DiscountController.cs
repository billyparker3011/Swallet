using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Services.Discount;

namespace SWallet.CustomerService.Controllers
{
    public class DiscountController : HnControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("static-discounts"), AllowAnonymous]
        public async Task<IActionResult> StaticDiscount()
        {
            return Ok(OkResponse.Create(await _discountService.GetStaticDiscount()));
        }
    }
}
