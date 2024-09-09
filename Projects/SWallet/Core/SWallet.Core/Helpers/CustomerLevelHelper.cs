using HnMicro.Core.Helpers;
using SWallet.Core.Enums;

namespace SWallet.Core.Helpers
{
    public static class CustomerLevelHelper
    {
        public static bool IsNormalLevel(this CustomerLevel level)
        {
            return level == CustomerLevel.Normal;
        }

        public static bool IsNormalLevel(this int level)
        {
            return level == CustomerLevel.Normal.ToInt();
        }

        public static bool IsBronzeLevel(this CustomerLevel level)
        {
            return level == CustomerLevel.Bronze;
        }

        public static bool IsBronzeLevel(this int level)
        {
            return level == CustomerLevel.Bronze.ToInt();
        }

        public static bool IsSilverLevel(this CustomerLevel level)
        {
            return level == CustomerLevel.Silver;
        }

        public static bool IsSilverLevel(this int level)
        {
            return level == CustomerLevel.Silver.ToInt();
        }

        public static bool IsGoldLevel(this CustomerLevel level)
        {
            return level == CustomerLevel.Gold;
        }

        public static bool IsGoldLevel(this int level)
        {
            return level == CustomerLevel.Gold.ToInt();
        }

        public static bool IsDiamondLevel(this CustomerLevel level)
        {
            return level == CustomerLevel.Diamond;
        }

        public static bool IsDiamondLevel(this int level)
        {
            return level == CustomerLevel.Diamond.ToInt();
        }
    }
}
