using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Payment;
using SWallet.Core.Services.Payments;
using SWallet.CustomerService.Requests.Payment;

namespace SWallet.CustomerService.Controllers
{
    public class PaymentController : HnControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            return Ok(OkResponse.Create(await _paymentService.GetPaymentMethods()));
        }

        [HttpGet("deposit-banks")]
        public async Task<IActionResult> GetBanksForDeposit([FromQuery] string paymentMethodCode)
        {
            return Ok(OkResponse.Create(await _paymentService.GetBanksForDeposit(paymentMethodCode)));
        }

        [HttpGet("deposit-bank-accounts")]
        public async Task<IActionResult> GetBankAccountsForDeposit([FromQuery] string paymentMethodCode, [FromQuery] int bankId)
        {
            return Ok(OkResponse.Create(await _paymentService.GetBankAccountsForDeposit(paymentMethodCode, bankId)));
        }

        [HttpGet("payment-content")]
        public async Task<IActionResult> GetPaymentContent([FromQuery] string paymentMethodCode)
        {
            return Ok(OkResponse.Create(await _paymentService.GetPaymentContent(paymentMethodCode)));
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            await _paymentService.Deposit(new DepositActivityModel
            {
                CustomerBankAccountId = request.CustomerBankAccountId,
                BankId = request.BankId,
                BankAccountId = request.BankAccountId,
                PaymentMethodCode = request.PaymentMethodCode,
                Amount = request.Amount,
                Content = request.Content
            });
            return Ok();
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        {
            await _paymentService.Withdraw(new WithdrawActivityModel
            {
                CustomerBankAccountId = request.CustomerBankAccountId,
                PaymentMethodCode = request.PaymentMethodCode,
                Amount = request.Amount
            });
            return Ok();
        }
    }
}
