using Lottery.Core.Attributes;

namespace Lottery.Core.Enums
{
    public enum SubCategory
    {
        #region For First Northern
        [EnumSubCategoryDescription("Đề", 1, Category.FirstNorthern, BetKind.None, new[] { BetKind.FirstNorthern_Northern_De, BetKind.FirstNorthern_Northern_DeTruot, BetKind.FirstNorthern_Northern_DeDau, BetKind.FirstNorthern_Northern_DeGiai1, BetKind.FirstNorthern_Northern_DeDauGiai1, BetKind.FirstNorthern_Northern_DeThanTai, BetKind.FirstNorthern_Northern_DeDauThanTai })]
        De = 0,

        [EnumSubCategoryDescription("Lô", 2, Category.FirstNorthern, BetKind.None, new[] { BetKind.FirstNorthern_Northern_Lo, BetKind.FirstNorthern_Northern_LoXien, BetKind.FirstNorthern_Northern_LoTruot, BetKind.FirstNorthern_Northern_LoDau })]
        Lo = 1,
        #endregion

        #region For Second Northern
        [EnumSubCategoryDescription("2D", 1, Category.SecondNorthern, BetKind.None, new[] { BetKind.SecondNorthern_Northern_2DDau, BetKind.SecondNorthern_Northern_2DDuoi })]
        D2 = 3,

        [EnumSubCategoryDescription("3D", 2, Category.SecondNorthern, BetKind.None, new[] { BetKind.SecondNorthern_Northern_3DDau, BetKind.SecondNorthern_Northern_3DDuoi, BetKind.SecondNorthern_Northern_3D23Lo })]
        D3 = 4,

        [EnumSubCategoryDescription("4D", 2, Category.SecondNorthern, BetKind.None, new[] { BetKind.SecondNorthern_Northern_4DDuoi, BetKind.SecondNorthern_Northern_4D20Lo })]
        D4 = 5,
        #endregion

        #region For Central
        [EnumSubCategoryDescription("2D", 1, Category.Central, BetKind.None, new[] { BetKind.Central_2DDau, BetKind.Central_2DDuoi, BetKind.Central_2D18Lo, BetKind.Central_2D18LoDau, BetKind.Central_2D7Lo })]
        CentralD2 = 6,

        [EnumSubCategoryDescription("3D", 2, Category.Central, BetKind.None, new[] { BetKind.Central_3DDau, BetKind.Central_3DDuoi, BetKind.Central_3D17Lo, BetKind.Central_3D7Lo })]
        CentralD3 = 7,

        [EnumSubCategoryDescription("4D", 3, Category.Central, BetKind.None, new[] { BetKind.Central_4DDuoi, BetKind.Central_4D16Lo })]
        CentralD4 = 8,

        [EnumSubCategoryDescription("Xiên", 4, Category.Central, BetKind.Central_LoXien)]
        CentralMixed = 9,

        [EnumSubCategoryDescription("Xiên 18A+B", 5, Category.Central, BetKind.Central_Mixed_LoXien)]
        CentralMixed18AB = 10,
        #endregion

        #region For Southern
        [EnumSubCategoryDescription("2D", 1, Category.Southern, BetKind.None, new[] { BetKind.Southern_2DDau, BetKind.Southern_2DDuoi, BetKind.Southern_2D18Lo, BetKind.Southern_2D18LoDau, BetKind.Southern_2D7Lo })]
        SouthernD2 = 11,

        [EnumSubCategoryDescription("3D", 2, Category.Southern, BetKind.None, new[] { BetKind.Southern_3DDau, BetKind.Southern_3DDuoi, BetKind.Southern_3D17Lo, BetKind.Southern_3D7Lo })]
        SouthernD3 = 12,

        [EnumSubCategoryDescription("4D", 3, Category.Southern, BetKind.None, new[] { BetKind.Southern_4DDuoi, BetKind.Southern_4D16Lo })]
        SouthernD4 = 13,

        [EnumSubCategoryDescription("Xiên", 4, Category.Southern, BetKind.Southern_LoXien)]
        SouthernMixed = 14,

        [EnumSubCategoryDescription("Xiên 18A+B", 5, Category.Southern, BetKind.Southern_Mixed_LoXien)]
        SouthernMixed18AB = 14
        #endregion
    }
}
