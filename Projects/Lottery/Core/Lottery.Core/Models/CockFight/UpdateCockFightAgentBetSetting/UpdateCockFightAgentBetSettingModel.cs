namespace Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting
{
    public class UpdateCockFightAgentBetSettingModel
    {
        public int BetKindId { get; set; }
        public decimal MainLimitAmountPerFight { get; set; }
        public decimal DrawLimitAmountPerFight { get; set; }
        public decimal LimitNumTicketPerFight { get; set; }
    }
}
