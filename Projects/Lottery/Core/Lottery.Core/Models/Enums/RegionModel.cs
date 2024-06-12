using Lottery.Core.Enums;

namespace Lottery.Core.Models.Enums
{
    public class RegionModel
    {
        public Region Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int NoOfPrize { get; set; }
    }
}
