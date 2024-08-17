using HnMicro.Core.Attributes;

namespace Lottery.Core.Enums.Partner.CockFight
{
    public enum CockFightTicketStatus
    {
        [EnumDescription("Ticket is confirmed")]
        TicketIsConfirm = 0,
        [EnumDescription("Settlement is in progress")]
        SettlementIsInProgress = 1,
        [EnumDescription("Settled but is waiting for payout")]
        SettledButIsWaitingForPayout = 2,
        [EnumDescription("Settled but payout is in progress")]
        SettledButPayoutIsInProgress = 3,
        [EnumDescription("Ticket is settled")]
        TicketIsSettled = 4,
        [EnumDescription("Canceled but is waiting for payout")]
        CanceledButIsWaitingForPayout = 5,
        [EnumDescription("Canceled but payout is in progress")]
        CanceledButPayoutIsInProgress = 6,
        [EnumDescription("Ticket is canceled")]
        TicketIsCanceled = 7,
        [EnumDescription("Void is in progress")]
        VoidIsInProgress = 8,
        [EnumDescription("Ticket is voided")]
        TicketIsVoided = 9,
        [EnumDescription("Unsettled but waiting for payout")]
        UnsettledButWaitingForPayout = 10,
        [EnumDescription("Unsettled but payout is in progress")]
        UnsettledButPayoutIsInProgress = 11,
        [EnumDescription("Uncancelled waiting for payout")]
        UncancelledWaitingForPayout = 12,
        [EnumDescription("Uncancelled but payout is in progress")]
        UncancelledButPayoutIsInProgress = 13
    }
}
