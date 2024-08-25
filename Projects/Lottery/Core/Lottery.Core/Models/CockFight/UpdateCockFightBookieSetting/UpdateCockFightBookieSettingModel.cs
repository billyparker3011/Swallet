namespace Lottery.Core.Models.CockFight.UpdateCockFightBookieSetting
{
    public class UpdateCockFightBookieSettingModel
    {
        public string ApiAddress { get; set; }
        public string PrivateKey { get; set; }
        public string PartnerAccountId { get; set; }
        public string GameClientId { get; set; }
        public string AuthValue { get; set; }
        public bool AllowScanByRange { get; set; }
        public DateTime? FromScanByRange { get; set; }
        public DateTime? ToScanByRange { get; set; }
        public bool IsMaintainance { get; set; }
    }
}
