using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Services.Payments;
using SWallet.Core.Services.Setting;

namespace SWallet.CustomerService.Controllers
{
    public class SettingController : HnControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;

        public SettingController(ISettingService settingService, IPaymentService paymentService)
        {
            _settingService = settingService;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSetting()
        {
            return Ok(OkResponse.Create(await _settingService.GetSetting()));
        }

        [HttpGet("payment-partner")]
        public async Task<IActionResult> GetPartnerPayment()
        {
            return Ok(OkResponse.Create(await _paymentService.GetActualPaymentPartner()));
        }
    }
}
