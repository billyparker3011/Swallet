namespace Lottery.Core.Models.Outs
{
    public class PlayerOutsModel
    {
        public decimal OutsByMatch { get; set; }
        public Dictionary<int, decimal> PointsByMatchAndNumbers { get; set; }
    }
}
