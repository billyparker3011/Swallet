using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Discounts;
using SWallet.Core.Services.Discount;
using SWallet.ManagerService.Requests.Discount;

namespace SWallet.ManagerService.Controllers
{
    public class DiscountController : HnControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscounts([FromQuery] GetDiscountsRequest request)
        {
            return Ok(OkResponse.Create(await _discountService.GetDiscounts(new GetDiscountsModel
            {
                Keyword = request.Keyword,
                IsStatic = request.IsStatic,
                SportKindId = request.SportKindId,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            })));
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] AddOrUpdateDiscountRequest request)
        {
            await _discountService.AddOrUpdateDiscount(new AddOrUpdateDiscountModel
            {
                EndedDate = request.EndedDate,
                StartedDate = request.StartedDate,
                Description = request.Description,
                IsEnabled = request.IsEnabled,
                IsStatic = request.IsStatic,
                Name = request.Name,
                SportKindId = request.SportKindId,
                Setting = request.Setting != null ? new DiscountSettingModel
                {
                    Deposit = request.Setting.Deposit != null
                        ? new DiscountDepositSettingModel
                        {
                            Amount = request.Setting.Deposit.Amount,
                            AmountType = request.Setting.Deposit.AmountType,
                            NoOfApplyDiscount = request.Setting.Deposit.NoOfApplyDiscount
                        }
                        : null,
                    Withdraw = request.Setting.Withdraw != null
                        ? new DiscountWithdrawSettingModel
                        {
                            MinDepositAmount = request.Setting.Withdraw.MinDepositAmount,
                            NoOfTickets = request.Setting.Withdraw.NoOfTickets
                        }
                        : null
                } : null
            });
            return Ok();
        }

        [HttpPut("{discountId:int}")]
        public async Task<IActionResult> UpdateDiscount([FromRoute] int discountId, [FromBody] AddOrUpdateDiscountRequest request)
        {
            await _discountService.AddOrUpdateDiscount(new AddOrUpdateDiscountModel
            {
                DiscountId = discountId,
                EndedDate = request.EndedDate,
                StartedDate = request.StartedDate,
                Description = request.Description,
                IsEnabled = request.IsEnabled,
                IsStatic = request.IsStatic,
                Name = request.Name,
                SportKindId = request.SportKindId,
                Setting = request.Setting != null ? new DiscountSettingModel
                {
                    Deposit = request.Setting.Deposit != null
                        ? new DiscountDepositSettingModel
                        {
                            Amount = request.Setting.Deposit.Amount,
                            AmountType = request.Setting.Deposit.AmountType,
                            NoOfApplyDiscount = request.Setting.Deposit.NoOfApplyDiscount
                        }
                        : null,
                    Withdraw = request.Setting.Withdraw != null
                        ? new DiscountWithdrawSettingModel
                        {
                            MinDepositAmount = request.Setting.Withdraw.MinDepositAmount,
                            NoOfTickets = request.Setting.Withdraw.NoOfTickets
                        }
                        : null
                } : null
            });
            return Ok();
        }

        [HttpPut("{discountId:int}/change-state")]
        public async Task<IActionResult> ChangeState([FromRoute] int discountId)
        {
            await _discountService.ChangeState(discountId);
            return Ok();
        }
    }
}