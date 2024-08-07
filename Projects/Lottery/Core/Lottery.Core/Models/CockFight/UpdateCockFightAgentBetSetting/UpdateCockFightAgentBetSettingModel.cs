namespace Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting
{
    public class UpdateCockFightAgentBetSettingModel
    {
        public long BetKindId { get; set; }
        public decimal MainLimitAmountPerFight { get; set; }
        public decimal DrawLimitAmountPerFight { get; set; }
        public decimal LimitNumTicketPerFight { get; set; }
    }
}
