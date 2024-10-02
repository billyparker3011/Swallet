using Lottery.Core.Enums;

namespace Lottery.Core.Models.Enums
{
    public class EnumSubCategoryInformation<T> where T : Enum
    {
        public string Name { get; set; }
        public int OrderBy { get; set; }
        public Category Category { get; set; }
        public T Value { get; set; }
        public Core.Enums.BetKind BetKind { get; set; }
        public List<Core.Enums.BetKind> SubBetKinds { get; set; }
    }
}
