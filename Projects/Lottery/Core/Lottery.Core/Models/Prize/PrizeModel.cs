namespace Lottery.Core.Models.Prize
{
    public class PrizeModel
    {
        public int Id { get; set; }
        public int PrizeId { get; set; }
        public string Name { get; set; }
        public int RegionId { get; set; }
        public int Order { get; set; }
        public int NoOfNumbers { get; set; }
    }
}
