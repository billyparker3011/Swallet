namespace Lottery.Core.Helpers
{
    public static class MixedHelper
    {
        public static void GenerateCombination(this int[] items, int noOfElements, List<List<int>> subsets)
        {
            var totalSubsets = 1 << items.Length;   // Total number of subsets is 2 ^ n
            for (var bitmask = 0; bitmask < totalSubsets; bitmask++)
            {
                // Count the number of set bits in the bitmask
                var count = 0;
                var temp = bitmask;
                while (temp > 0)
                {
                    count += temp & 1;
                    temp >>= 1;
                }

                if (count == noOfElements)
                {
                    var subset = new int[noOfElements];
                    var index = 0;
                    for (var i = 0; i < items.Length; i++)
                    {
                        if ((bitmask & (1 << i)) > 0) subset[index++] = items[i];
                    }
                    subsets.Add(subset.ToList());
                }
            }
        }
    }
}
