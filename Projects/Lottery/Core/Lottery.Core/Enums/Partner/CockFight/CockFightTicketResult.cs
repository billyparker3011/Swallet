using HnMicro.Core.Attributes;

namespace Lottery.Core.Enums.Partner.CockFight
{
    public enum CockFightTicketResult
    {
        [EnumDescription("Ticket is settled as win")]
        Win,
        [EnumDescription("Ticket is settled as loss")]
        Loss,
        [EnumDescription("Ticket is settled as draw")]
        Draw
    }
}
