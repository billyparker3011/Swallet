namespace Lottery.Core.Models.Ticket
{
    public class RefundRejectTicketResultModel
    {
        public decimal DifferentPlayerPayout { get; set; }
        public Dictionary<int, decimal> OutsByNumbers { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> PointsByNumbers { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> OutsByBetKind { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> PointsByBetKind { get; set; } = new Dictionary<int, decimal>();
    }
}
