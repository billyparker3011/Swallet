namespace Lottery.Core.Models.Navigation
{
    public class SubNavigationDetailModel
    {
        public string Name { get; set; }
        public int BetKindId { get; set; }
        public int? ReplacedById { get; set; }
        public bool Display { get; set; }
        public bool Enabled { get; set; }
        public int? NoOfRemainingNumbers { get; set; }
        public List<CorrelationBetKindModel> Correlations { get; set; }
    }
}
