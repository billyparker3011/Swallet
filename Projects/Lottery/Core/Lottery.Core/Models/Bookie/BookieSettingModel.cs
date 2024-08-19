using Lottery.Core.Enums.Partner;

namespace Lottery.Core.Models.Bookie
{
    public class BookieSettingModel
    {
        public int Id { get; set; }
        public PartnerType BookieTypeId { get; set; }
        public string SettingValue { get; set; }
    }
}
