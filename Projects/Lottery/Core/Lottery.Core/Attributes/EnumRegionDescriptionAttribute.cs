namespace Lottery.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumRegionDescriptionAttribute : Attribute
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int NoOfPrize { get; set; }

        public EnumRegionDescriptionAttribute(string code, string name, int noOfPrize)
        {
            Code = code;
            Name = name;
            NoOfPrize = noOfPrize;
        }
    }
}
