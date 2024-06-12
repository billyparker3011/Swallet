using Lottery.Core.Enums;

namespace Lottery.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumCategoryDescriptionAttribute : Attribute
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Region Region { get; set; }
        public int OrderBy { get; set; }

        public EnumCategoryDescriptionAttribute(string code, string name, Region region, int orderBy)
        {
            Code = code;
            Name = name;
            Region = region;
            OrderBy = orderBy;
        }
    }
}
