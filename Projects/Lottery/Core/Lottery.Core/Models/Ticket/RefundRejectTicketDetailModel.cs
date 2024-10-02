using Lottery.Core.Enums;

namespace Lottery.Core.Models.Ticket
{
    public class RefundRejectTicketDetailModel
    {
        public long TicketId { get; set; }
        public string ChoosenNumbers { get; set; }
        public TicketState State { get; set; }
        public bool IsLive { get; set; }
        public decimal Stake { get; set; }
        public decimal PlayerPayout { get; set; }
        public decimal AgentPayout { get; set; }
        public decimal MasterPayout { get; set; }
        public decimal SupermasterPayout { get; set; }
        public decimal CompanyPayout { get; set; }
    }
}
