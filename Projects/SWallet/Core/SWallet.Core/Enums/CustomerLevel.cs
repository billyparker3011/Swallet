using SWallet.Core.Attributes;
using SWallet.Core.Consts;

namespace SWallet.Core.Enums
{
    public enum CustomerLevel
    {
        [EnumCustomerLevel(LevelConsts.LevelAsNormalCode)]
        Normal = 1,

        [EnumCustomerLevel(LevelConsts.LevelAsBronzeCode)]
        Bronze = 20,

        [EnumCustomerLevel(LevelConsts.LevelAsSilverCode)]
        Silver = 30,

        [EnumCustomerLevel(LevelConsts.LevelAsGoldCode)]
        Gold = 40,

        [EnumCustomerLevel(LevelConsts.LevelAsDiamondCode)]
        Diamond = 50
    }
}
