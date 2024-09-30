using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Payment;
using SWallet.Core.Services.Payments;
using SWallet.ManagerService.Requests.Payment;

namespace SWallet.ManagerService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.PaymentMethod.BaseRoute)]
    public class PaymentMethodController : HnControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentMethodController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{paymentPartnerId:int}")]
        public async Task<IActionResult> GetPaymentMethods([FromRoute] int paymentPartnerId)
        {
            return Ok(OkResponse.Create(await _paymentService.GetPaymentMethodsBy(paymentPartnerId)));
        }

        [HttpPost("{paymentPartnerId:int}")]
        public async Task<IActionResult> AddPaymentMethod([FromRoute] int paymentPartnerId, [FromBody] CreatePaymentMethodRequest request)
        {
            await _paymentService.CreatePaymentMethod(new CreatePaymentMethodModel
            {
                PaymentPartnerId = paymentPartnerId,
                Name = request.Name,
                Code = request.Code,
                Enabled = request.Enabled,
                Fee = request.Fee,
                Max = request.Max,
                Min = request.Min,
                Icon = request.Icon
            });
            return Ok();
        }
    }
}