using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Bti;


namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiTicketService : IScopedDependency
    {
        Task<BtiValidateTokenResponseModel> ValidateToken(string token);

        Task<BtiDebitReserveResponseModel> Reverse(string cust_id, long reserve_id, decimal amount, string extsessionID, string requestBody);

        Task<BtiDebitReserveResponseModel> DebitReverse(string cust_id, long reserve_id, decimal amount, long req_id, long purchase_id, string requestBody);

        Task<BtiBaseResponseModel> CancelReverse(string cust_id, long reserve_id);

        Task<BtiBaseResponseModel> CommitReverse(string cust_id, long reserve_id, long purchase_id);

        Task<BtiBaseResponseModel> DebitCustomer(string cust_id, decimal amount, long req_id, long purchase_id, string requestBody);

        Task<BtiBaseResponseModel> CreditCustomer(string cust_id, decimal amount, long req_id, long purchase_id, string requestBody);
    }
}
