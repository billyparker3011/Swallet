using Lottery.Core.Enums.Partner;

namespace Lottery.Core.Partners.Models.Ga28
{
    public class Ga28SyncUpBetSettingModel : IBaseMessageModel
    {
        public string MemberRefId { get; set; }
        public string AccountId { get; set; }
        public decimal? MainLimitAmountPerFight { get; set; }
        public decimal? DrawLimitAmountPerFight { get; set; }
        public decimal? LimitNumTicketPerFight { get; set; }
        public PartnerType Partner { get; set; } = PartnerType.GA28;
    }
}
