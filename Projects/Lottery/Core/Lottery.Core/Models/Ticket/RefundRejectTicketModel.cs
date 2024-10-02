namespace Lottery.Core.Models.Ticket
{
    public class RefundRejectTicketModel
    {
        public RefundRejectTicketDetailModel Ticket { get; set; }
        public List<RefundRejectTicketDetailModel> Children { get; set; }
    }
}
