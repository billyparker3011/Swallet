namespace Lottery.Core.Partners.Models.Allbet
{
    public class AllbetBookieSettingValue
    {
        public string Agent { get; set; }
        public string ApiURL { get; set; }
        public string ContentType { get; set; }
        public string OperatorId { get; set; }
        public string AllbetApiKey { get; set; }
        public string PartnerApiKey { get; set; }
        public string Suffix { get; set; }
        public bool AllowScanByRange { get; set; }
        public DateTime? FromScanByRange { get; set; }
        public DateTime? ToScanByRange { get; set; }
        public bool IsMaintainance { get; set; }
        public DateTime? FromMaintainance { get; set; }
        public DateTime? ToMaintainance { get; set; }
    }
}
