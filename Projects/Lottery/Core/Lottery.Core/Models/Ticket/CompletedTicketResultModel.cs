using Lottery.Core.Enums;

namespace Lottery.Core.Models.Ticket
{
    public class CompletedTicketResultModel
    {
        public long TicketId { get; set; }
        public TicketState State { get; set; }
        public int? Times { get; set; }
        public string MixedTimes { get; set; }

        public decimal PlayerWinLose { get; set; }
        public decimal DraftPlayerWinLose { get; set; }

        public decimal AgentWinLose { get; set; }
        public decimal AgentCommission { get; set; }
        public decimal DraftAgentWinLose { get; set; }
        public decimal DraftAgentCommission { get; set; }

        public decimal MasterWinLose { get; set; }
        public decimal MasterCommission { get; set; }
        public decimal DraftMasterWinLose { get; set; }
        public decimal DraftMasterCommission { get; set; }

        public decimal SupermasterWinLose { get; set; }
        public decimal SupermasterCommission { get; set; }
        public decimal DraftSupermasterWinLose { get; set; }
        public decimal DraftSupermasterCommission { get; set; }

        public decimal CompanyWinLose { get; set; }
        public decimal DraftCompanyWinLose { get; set; }

        public List<CompletedChildrenTicketResultModel> Children { get; set; } = new List<CompletedChildrenTicketResultModel>();
    }
}
