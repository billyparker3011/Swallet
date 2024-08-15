namespace Lottery.Core.Enums.Partner.CockFight
{
    public enum CockFightTicketStatus
    {
        TicketIsConfirm = 0,
        SettlementIsInProgress = 1,
        SettledButIsWaitingForPayout = 2,
        SettledButPayoutIsInProgress = 3,
        TicketIsSettled = 4,
        CanceledButIsWaitingForPayout = 5,
        CanceledButPayoutIsInProgress = 6,
        TicketIsCanceled = 7,
        VoidIsInProgress = 8,
        TicketIsVoided = 9,
        UnsettledButWaitingForPayout = 10,
        UnsettledButPayoutIsInProgress = 11,
        UncancelledWaitingForPayout = 12,
        UncancelledButPayoutIsInProgress = 13
    }
}
