namespace Lottery.Core.Models.Navigation
{
    public class SubNavigationModel
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public int? BetKindId { get; set; }
        public int RegionId { get; set; }
        public List<SubNavigationDetailModel> Children { get; set; }
        public List<CorrelationBetKindModel> Correlations { get; set; }
    }
}
