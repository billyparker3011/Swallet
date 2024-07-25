namespace Lottery.Core.Models.Outs
{
    public class MixedPlayerOutsModel
    {
        public decimal OutsByMatch { get; set; }
        //  Key = BetKindId; Value = Dict<Key = Number; Value = Point>
        public Dictionary<int, Dictionary<int, decimal>> PointsByMatchAndNumbers { get; set; } = new Dictionary<int, Dictionary<int, decimal>>();
    }
}
