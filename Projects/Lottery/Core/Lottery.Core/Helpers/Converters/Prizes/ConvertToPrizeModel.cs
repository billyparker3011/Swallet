using Lottery.Core.Models.Prize;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.Prizes
{
    public static class ConvertToPrizeModel
    {
        public static PrizeModel ToPrizeModel(this Prize prize)
        {
            return new PrizeModel
            {
                Id = prize.Id,
                Name = prize.Name,
                NoOfNumbers = prize.NoOfNumbers,
                Order = prize.Order,
                PrizeId = prize.PrizeId,
                RegionId = prize.RegionId
            };
        }
    }
}
