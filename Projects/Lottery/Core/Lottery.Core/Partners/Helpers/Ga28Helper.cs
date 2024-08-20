using HnMicro.Core.Helpers;
using Lottery.Core.Enums.Partner.CockFight;

namespace Lottery.Core.Partners.Helpers
{
    public static class Ga28Helper
    {
        public static int ToTicketResult(this string result)
        {
            if (string.IsNullOrEmpty(result)) return default;
            return result switch
            {
                "win" => CockFightTicketResult.Win.ToInt(),
                "loss" => CockFightTicketResult.Loss.ToInt(),
                "draw" => CockFightTicketResult.Draw.ToInt(),
                _ => default,
            };
        }
    }
}
