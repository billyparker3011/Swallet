namespace Lottery.Core.Models.Ticket
{
    public class RefundRejectTicketResultModel
    {
        public bool Allow { get; set; }
        public RefundRejectTicketDetailModel Ticket { get; set; } = new RefundRejectTicketDetailModel();
        public List<RefundRejectTicketDetailModel> Children { get; set; } = new List<RefundRejectTicketDetailModel>();
    }
}
