﻿using HnMicro.Core.Helpers;
using Lottery.Core.Enums;

namespace Lottery.Core.Helpers
{
    public static class CommonHelper
    {
        public static bool IsSuspended(this UserState state)
        {
            return state == UserState.Suspended;
        }

        public static bool IsClosed(this UserState state)
        {
            return state == UserState.Closed;
        }

        public static int GetPositionOfPrize(this int prize, int position)
        {
            return 10 * prize + position;
        }

        public static int GetDefaultPositionOfPrize()
        {
            return 1.GetPositionOfPrize(0);
        }

        public static List<int> OutsTicketState()
        {
            return new List<int> { TicketState.Running.ToInt(), TicketState.Waiting.ToInt() };
        }

        public static List<int> CompletedTicketState()
        {
            return new List<int> { TicketState.Completed.ToInt(), TicketState.Refund.ToInt(), TicketState.Reject.ToInt(), TicketState.Won.ToInt(), TicketState.Draw.ToInt(), TicketState.Lose.ToInt() };
        }
    }
}
