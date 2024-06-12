namespace Lottery.Core.Models.Enums
{
    public class EnumRegionInformation<T> where T : Enum
    {
        public T Value { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int NoOfPrize { get; set; }
    }
}
