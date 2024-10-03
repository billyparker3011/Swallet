﻿using SWallet.Core.Enums;

namespace SWallet.Core.Models
{
    public class CustomerSessionModel
    {
        public long CustomerId { get; set; }
        public string Hash { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
        public string UserAgent { get; set; }
        public SessionState State { get; set; }
        public DateTime? LatestDoingTime { get; set; }
    }
}