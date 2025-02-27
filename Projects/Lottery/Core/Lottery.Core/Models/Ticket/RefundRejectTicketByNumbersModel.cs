﻿namespace Lottery.Core.Models.Ticket
{
    public class RefundRejectTicketByNumbersModel
    {
        public List<string> RefundRejectNumbers { get; set; } = new List<string>();
        public RefundRejectTicketDetailModel Ticket { get; set; }
        public List<RefundRejectTicketDetailModel> Children { get; set; }
    }
}
