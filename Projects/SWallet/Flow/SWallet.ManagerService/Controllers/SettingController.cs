using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Settings;
using SWallet.Core.Services.Payments;
using SWallet.Core.Services.Setting;
using SWallet.ManagerService.Requests.Setting;

namespace SWallet.ManagerService.Controllers
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

        [HttpPost]
        public async Task<IActionResult> UpdateSetting([FromBody] UpdateSettingRequest request)
        {
            await _settingService.UpdateSetting(new SettingModel
            {
                Currency = new CurrencySettingModel
                {
                    CurrencySymbol = request.Currency.CurrencySymbol
                },
                Mask = new MaskSettingModel
                {
                    MaskCharacter = request.Mask.MaskCharacter,
                    NumberOfMaskCharacters = request.Mask.NumberOfMaskCharacters
                },
                PaymentPartner = request.PaymentPartner
            });
            return Ok();
        }

        [HttpGet("payment-partners")]
        public IActionResult GetPaymentPartners()
        {
            return Ok(OkResponse.Create(_paymentService.GetPaymentPartners()));
        }
    }
}