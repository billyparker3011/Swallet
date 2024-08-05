using HnMicro.Core.Attributes;

namespace Lottery.Core.Enums.Partner.CockFight
{
    public enum CockFightTicketResult
    {
        [EnumDescription("win")]
        Win,
        [EnumDescription("loss")]
        Loss,
        [EnumDescription("draw")]
        Draw
    }
}
