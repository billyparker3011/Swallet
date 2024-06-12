using Lottery.Core.Enums;

namespace Lottery.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumSubCategoryDescriptionAttribute : Attribute
    {
        public string Name { get; set; }
        public int OrderBy { get; set; }
        public Category Category { get; set; }
        public BetKind BetKind { get; set; }
        public List<BetKind> SubBetKind { get; set; }

        public EnumSubCategoryDescriptionAttribute(string name, int orderBy, Category category, BetKind betKind, params BetKind[] subBetKinds)
        {
            Name = name;
            OrderBy = orderBy;
            Category = category;
            BetKind = betKind;
            SubBetKind = subBetKinds.ToList();
        }
    }
}
