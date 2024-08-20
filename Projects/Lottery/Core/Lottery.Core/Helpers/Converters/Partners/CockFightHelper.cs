using Lottery.Core.Models.BetKind;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Helpers.Converters.Partners
{
    public static class CockFightHelper
    {
        public static CockFightBetKindModel ToCockFightBetKindModel(this CockFightBetKind cockFightBetKind)
        {
            return new CockFightBetKindModel
            {
                Id = cockFightBetKind.Id,
                Name = cockFightBetKind.Name
            };
        }
    }
}
