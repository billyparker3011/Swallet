namespace Lottery.Core.Models.Ticket
{
    public class RefundRejectTicketByNumbersResultModel
    {
        public bool Allow { get; set; }
        public decimal DifferentPlayerPayout { get; set; }
        public RefundRejectTicketDetailModel Ticket { get; set; } = new RefundRejectTicketDetailModel();
        public List<RefundRejectTicketDetailModel> Children { get; set; } = new List<RefundRejectTicketDetailModel>();
        public Dictionary<int, decimal> OutsByNumbers { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> PointsByNumbers { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> OutsByBetKind { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> PointsByBetKind { get; set; } = new Dictionary<int, decimal>();
    }
}
