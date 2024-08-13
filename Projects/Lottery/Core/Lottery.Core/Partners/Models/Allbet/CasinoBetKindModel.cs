namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoBetKindModel
    {
        public long Id { get; set; }
        public long BookieId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string RegionId { get; set; }
        public string CategoryId { get; set; }
        public bool? IsLive { get; set; }
        public int? OrderInCategory { get; set; }
        public decimal? Award { get; set; }
        public bool? Enabled { get; set; }
    }
}
