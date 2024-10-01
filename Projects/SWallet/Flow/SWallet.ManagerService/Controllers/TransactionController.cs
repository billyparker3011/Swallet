using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Transactions;
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

        [HttpGet("get-customer-transactions")]
        public async Task<IActionResult> GetAllCustomerTransactions([FromQuery] CustomerTransactionsRequest request, [FromQuery] QueryAdvance advanceRequest)
        {
            return Ok(OkResponse.Create(await _transactionService.GetTransactionsHistory(new Core.Models.Transactions.GetTransactionsHistoryModel
            {
                TransactionType = request.TransactionType,
                From = request.From,
                To = request.To,
                State = request.State,
                PageIndex = advanceRequest.PageIndex,
                PageSize = advanceRequest.PageSize,
                SortName = advanceRequest.SortName,
                SortType = advanceRequest.SortType,
                GetAllCustomerTrans = true
            })));
        }

        [HttpPut("{transactionId:long}/completed")]
        public async Task<IActionResult> CompletedTransaction([FromRoute] long transactionId, [FromBody] CompletedTransactionRequest request)
        {
            await _transactionService.CompletedTransaction(new CompletedTransactionModel
            {
                TransactionId = transactionId,
                Amount = request.Amount
            });
            return Ok();
        }

        [HttpGet("{transactionId:long}/rejected")]
        public async Task<IActionResult> RejectedTransaction([FromRoute] long transactionId)
        {
            return Ok(OkResponse.Create(await _transactionService.RejectedTransaction(transactionId)));
        }
    }
}