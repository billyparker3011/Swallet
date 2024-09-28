using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Transactions;
using SWallet.Core.Services.Transaction;
using SWallet.CustomerService.Validations.Transactions;

namespace SWallet.CustomerService.Controllers
{
    public class TransactionController : HnControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsHistory([FromQuery] GetTransactionsHistoryRequest request, [FromQuery] QueryAdvance advanceRequest)
        {
            return Ok(OkResponse.Create(await _transactionService.GetTransactionsHistory(new GetTransactionsHistoryModel
            {
                TransactionType = request.TransactionType,
                State = request.State,
                From = request.From,
                To = request.To,
                PageSize = advanceRequest.PageSize,
                PageIndex = advanceRequest.PageIndex,
                SortName = advanceRequest.SortName,
                SortType = advanceRequest.SortType
            })));
        }
    }
}
