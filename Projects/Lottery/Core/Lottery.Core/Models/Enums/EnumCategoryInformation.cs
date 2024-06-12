using Lottery.Core.Enums;

namespace Lottery.Core.Models.Enums
{
    public class EnumCategoryInformation<T> where T : Enum
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Region Region { get; set; }
        public int OrderBy { get; set; }
        public T Value { get; set; }
    }
}
