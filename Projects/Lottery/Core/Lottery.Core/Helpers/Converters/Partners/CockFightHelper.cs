using Lottery.Core.Enums.Partner.CockFight;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Partners.Enums;
using Lottery.Data.Entities.Partners.CockFight;
using System;

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

        public static CockFightSelection ConvertTicketSelection(this string selection)
        {
            switch (selection)
            {
                case "player":
                    return CockFightSelection.Player;
                case "draw":
                    return CockFightSelection.Draw;
                case "bank":
                    return CockFightSelection.Banker;
                default:
                    return CockFightSelection.Unknown;
            }
        }

        public static CockFightTicketResult ConvertTicketResult(this int result)
        {
            return (CockFightTicketResult)Enum.ToObject(typeof(CockFightTicketResult), result);
        }

        public static CockFightTicketStatus ConvertTicketStatus(this int status)
        {
            return (CockFightTicketStatus)Enum.ToObject(typeof(CockFightTicketStatus), status);
        }
    }
}
