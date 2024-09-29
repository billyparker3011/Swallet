using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Services.Transaction;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Controllers
{
    public class TransactionController : HnControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("customer/{id:long}/transactions")]
        public async Task<IActionResult> GetCustomerTransactions([FromRoute] long id, [FromQuery] CustomerTransactionsRequest request, [FromQuery] QueryAdvance advanceRequest)
        {
            return Ok(OkResponse.Create(await _transactionService.GetTransactionsHistory(new Core.Models.Transactions.GetTransactionsHistoryModel
            {
                CustomerId = id,
                TransactionType = request.TransactionType,
                From = request.From,
                To = request.To,
                State = request.State,
                PageIndex = advanceRequest.PageIndex,
                PageSize = advanceRequest.PageSize,
                SortName = advanceRequest.SortName,
                SortType = advanceRequest.SortType
            })));
        }
    }
}