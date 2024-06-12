using Lottery.Core.Attributes;

namespace Lottery.Core.Enums
{
    public enum Category
    {
        //  Mien Bac
        [EnumCategoryDescription("FirstNorthern_Category", "Miền Bắc 1", Region.Northern, 1)]
        FirstNorthern = 1,

        [EnumCategoryDescription("SecondNorthern_Category", "Miền Bắc 2", Region.Northern, 2)]
        SecondNorthern = 2,

        //  Mien Trung
        [EnumCategoryDescription("Central_Category", "Miền Trung", Region.Central, 3)]
        Central = 10,

        [EnumCategoryDescription("Central_18A_18B_Category", "Miền Trung 18A+B", Region.Central, 4)]
        Central18A18B = 11,

        //  Mien Nam
        [EnumCategoryDescription("Southern_18A_Category", "Miền Nam 18A", Region.Southern, 5)]
        Southern18A = 20,

        [EnumCategoryDescription("Southern_18B_Category", "Miền Nam 18B", Region.Southern, 5)]
        Southern18B = 21,

        [EnumCategoryDescription("Southern_18C_Category", "Miền Nam 18C", Region.Southern, 5)]
        Southern18C = 22,

        [EnumCategoryDescription("Southern_18A_18B_Category", "Miền Nam 18A+B", Region.Southern, 6)]
        Southern18A18B = 23
    }
}
