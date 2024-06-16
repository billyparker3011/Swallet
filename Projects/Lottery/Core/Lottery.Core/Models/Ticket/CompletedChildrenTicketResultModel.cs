using Lottery.Core.Enums;

namespace Lottery.Core.Models.Ticket
{
    public class CompletedChildrenTicketResultModel
    {
        public long TicketId { get; set; }
        public TicketState State { get; set; }
        public int? Times { get; set; }
        public string MixedTimes { get; set; }

        public decimal PlayerWinLoss { get; set; }

        public decimal AgentWinLoss { get; set; }
        public decimal AgentCommission { get; set; }

        public decimal MasterWinLoss { get; set; }
        public decimal MasterCommission { get; set; }

        public decimal SupermasterWinLoss { get; set; }
        public decimal SupermasterCommission { get; set; }

        public decimal CompanyWinLoss { get; set; }
    }
}
