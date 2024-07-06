namespace Lottery.Core.Helpers
{
    public static class StringLotteryHelper
    {
        public static string NormalizeNumber(this int number, int noOfNumbers = 100)
        {
            if (noOfNumbers == 1000) return number <= 9 ? $"00{number}" : number.ToString("N0");
            else if (noOfNumbers == 10000) return number <= 9 ? $"000{number}" : number.ToString("N0");
            return number <= 9 ? $"0{number}" : number.ToString("N0");
        }

        public static bool GetEndOfResult(this string rs, out string val, int noOfCharacter = 2)
        {
            val = string.Empty;
            if (string.IsNullOrEmpty(rs) || rs.Length < noOfCharacter) return false;
            val = rs.Substring(rs.Length - noOfCharacter);
            return true;
        }

        public static bool GetStartOfResult(this string rs, out string val, int noOfCharacter = 2)
        {
            val = string.Empty;
            if (string.IsNullOrEmpty(rs) || rs.Length < noOfCharacter) return false;
            val = rs.Substring(0, noOfCharacter);
            return true;
        }
    }
}
