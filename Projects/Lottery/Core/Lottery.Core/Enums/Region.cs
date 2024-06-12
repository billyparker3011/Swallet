using Lottery.Core.Attributes;

namespace Lottery.Core.Enums
{
    public enum Region
    {
        [EnumRegionDescription("Northern_Region", "Miền Bắc", 9)] //  Include Than Tai
        Northern = 1,

        [EnumRegionDescription("Central_Region", "Miền Trung", 9)]
        Central = 2,

        [EnumRegionDescription("Southern_Region", "Miền Nam", 9)]
        Southern = 3
    }
}
