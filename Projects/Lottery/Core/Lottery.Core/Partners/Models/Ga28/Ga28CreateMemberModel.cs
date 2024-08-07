using Lottery.Core.Enums.Partner;

namespace Lottery.Core.Partners.Models.Ga28
{
    public class Ga28CreateMemberModel : IBaseMessageModel
    {
        public string MemberRefId { get; set; }
        public string AccountId { get; set; }
        public long PlayerId { get; set; }
        public PartnerType Partner { get; set; } = PartnerType.GA28;
    }
}
