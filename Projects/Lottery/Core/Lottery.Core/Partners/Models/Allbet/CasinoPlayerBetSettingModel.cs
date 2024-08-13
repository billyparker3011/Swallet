namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoPlayerBetSettingModel
    {
        public long Id { get; set; }
        public long PlayerMappingId { get; set; }
        public long CABetKindId { get; set; }
        public long VipHandicapId { get; set; }
        public decimal? MinBet { get; set; }
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
    }
}
