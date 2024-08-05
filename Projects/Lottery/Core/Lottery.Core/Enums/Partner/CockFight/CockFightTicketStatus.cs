namespace Lottery.Core.Enums.Partner.CockFight
{
    public enum CockFightTicketStatus
    {
        TicketIsConfirm,
        SettlementIsInProgress,
        SettledButIsWaitingForPayout,
        SettledButPayoutIsInProgress,
        TicketIsSettled,
        CanceledButIsWaitingForPayout,
        CanceledButPayoutIsInProgress,
        TicketIsCanceled,
        VoidIsInProgress,
        TicketIsVoided,
        UnsettledButWaitingForPayout,
        UnsettledButPayoutIsInProgress,
        UncancelledWaitingForPayout,
        UncancelledButPayoutIsInProgress
    }
}
