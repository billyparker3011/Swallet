namespace Lottery.Core.Models.BetKind;

public class BetKindModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int RegionId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public bool IsLive { get; set; }
    public int? ReplaceByIdWhenLive { get; set; }
    public int OrderInCategory { get; set; }
    public decimal Award { get; set; }
    public bool Enabled { get; set; }
    public bool IsMixed { get; internal set; }
    public List<int> CorrelationBetKindIds { get; set; }
}
