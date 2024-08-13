namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentBetSettingModel
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public long CABetKindId { get; set; }
        public long DefaultVipHandicapId { get; set; }
        public decimal? MinBet { get; set; }
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
    }
}
