﻿namespace Lottery.Core.Partners.Models.Ga28
{
    public class Ga28BookieSettingValue
    {
        public string ApiAddress { get; set; }
        public string PrivateKey { get; set; }
        public string PartnerAccountId { get; set; }
        public string GameClientId { get; set; }
        public string AuthValue { get; set; }
        public string ApplicationStaticToken { get; set; }
        public string ScanTicketTime { get; set; }
        public bool AllowScanByRange { get; set; }
        public DateTime? FromScanByRange { get; set; }
        public DateTime? ToScanByRange { get; set; }
        public bool? IsMaintainance { get; set; }
        public DateTime? FromMaintainance { get; set; }
        public DateTime? ToMaintainance { get; set; }
    }
}
