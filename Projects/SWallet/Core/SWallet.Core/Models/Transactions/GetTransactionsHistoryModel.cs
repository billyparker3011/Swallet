using HnMicro.Framework.Models;

namespace SWallet.Core.Models.Transactions
{
    public class GetTransactionsHistoryModel : QueryAdvance
    {
        public int? TransactionType { get; set; }
        public int? State { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}
