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
        //  BetKind.SecondNorthern_Northern_2DDuoi
        [EnumSubCategoryDescription("2D", 1, Category.SecondNorthern, BetKind.None, new[] { BetKind.SecondNorthern_Northern_2DDau, BetKind.SecondNorthern_Northern_2D27Lo })]
        D2 = 3,

        [EnumSubCategoryDescription("3D", 2, Category.SecondNorthern, BetKind.None, new[] { BetKind.SecondNorthern_Northern_3DDau, BetKind.SecondNorthern_Northern_3DDuoi, BetKind.SecondNorthern_Northern_3D23Lo })]
        D3 = 4,

        [EnumSubCategoryDescription("4D", 2, Category.SecondNorthern, BetKind.None, new[] { BetKind.SecondNorthern_Northern_4DDuoi, BetKind.SecondNorthern_Northern_4D20Lo })]
        D4 = 5,
        #endregion

        #region For Southern
        [EnumSubCategoryDescription("2D", 1, Category.Southern18A, BetKind.None, new[] { BetKind.SecondNorthern_Southern_2DDau, BetKind.SecondNorthern_Southern_2DDuoi, BetKind.SecondNorthern_Southern_2D18Lo })]
        SouthernD2 = 6,

        [EnumSubCategoryDescription("3D", 2, Category.Southern18A, BetKind.None, new[] { BetKind.SecondNorthern_Southern_3DDau, BetKind.SecondNorthern_Southern_3DDuoi, BetKind.SecondNorthern_Southern_3D17Lo, BetKind.SecondNorthern_Southern_3D7Lo })]
        SouthernD3 = 7,

        [EnumSubCategoryDescription("4D", 2, Category.Southern18A, BetKind.None, new[] { BetKind.SecondNorthern_Southern_4DDuoi, BetKind.SecondNorthern_Southern_4D16Lo })]
        SouthernD4 = 8
        #endregion
    }
}
