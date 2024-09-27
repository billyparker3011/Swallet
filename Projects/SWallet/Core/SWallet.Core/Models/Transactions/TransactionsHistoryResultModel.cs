using HnMicro.Framework.Models;

namespace SWallet.Core.Models.Transactions
{
    public class TransactionsHistoryResultModel : BaseResult
    {
        public List<TransactionModel> Transactions { get; set; }
    }
}
