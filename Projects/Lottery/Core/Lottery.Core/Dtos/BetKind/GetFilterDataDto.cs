using Lottery.Core.Models.Enums;

namespace Lottery.Core.Dtos.BetKind
{
    public class GetFilterDataDto
    {
        public IEnumerable<RegionModel> Regions { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }
    }
}
