using Lottery.Core.Enums;

namespace Lottery.Core.Models.Enums
{
    public class SubCategoryModel
    {
        public SubCategory Id { get; set; }
        public string Name { get; set; }
        public int OrderBy { get; set; }
        public Category Category { get; set; }
        public Core.Enums.BetKind BetKind { get; set; }
        public List<Core.Enums.BetKind> SubBetKinds { get; set; }
    }
}
