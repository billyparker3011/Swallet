using Lottery.Core.Models.BetKind;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.BetKinds
{
    public static class ConvertToBetKindModel
    {
        public static BetKindModel ToBetKindModel(this BetKind betKind)
        {
            return new BetKindModel
            {
                Id = betKind.Id,
                Name = betKind.Name,
                RegionId = betKind.RegionId,
                CategoryId = betKind.CategoryId,
                CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Enums.Category)betKind.CategoryId)?.Name,
                IsLive = betKind.IsLive,
                ReplaceByIdWhenLive = betKind.ReplaceByIdWhenLive,
                OrderInCategory = betKind.OrderInCategory,
                Award = betKind.Award,
                Enabled = betKind.Enabled,
                IsMixed = betKind.IsMixed,
                CorrelationBetKindIds = string.IsNullOrEmpty(betKind.CorrelationBetKindIds) ? new List<int>() : betKind.CorrelationBetKindIds.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(f1 => int.Parse(f1)).ToList(),
            };
        }
    }
}
