using HnMicro.Core.Helpers;
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

        public static bool IsMixed(this int betKindId)
        {
            return betKindId == BetKind.FirstNorthern_Northern_LoXien.ToInt()
                || betKindId == BetKind.Central_LoXien.ToInt()
                || betKindId == BetKind.Central_Mixed_LoXien.ToInt()
                || betKindId == BetKind.Southern_LoXien.ToInt()
                || betKindId == BetKind.Southern_Mixed_LoXien.ToInt();
        }

        public static List<int> BuildBetKinds(this int betKindId)
        {
            //  Northern
            if (betKindId == BetKind.FirstNorthern_Northern_LoLive.ToInt())
                return new List<int> { BetKind.FirstNorthern_Northern_Lo.ToInt(), betKindId };

            if (betKindId == BetKind.FirstNorthern_Northern_LoXien.ToInt())
                return new List<int> { BetKind.FirstNorthern_Northern_Xien2.ToInt(), BetKind.FirstNorthern_Northern_Xien3.ToInt(), BetKind.FirstNorthern_Northern_Xien4.ToInt() };

            if (betKindId == BetKind.SecondNorthern_Northern_2DDau.ToInt())
                return new List<int> { BetKind.SecondNorthern_Northern_2DDuoi.ToInt(), betKindId };

            if (betKindId == BetKind.SecondNorthern_Northern_2DDuoi.ToInt())
                return new List<int> { BetKind.SecondNorthern_Northern_2DDau.ToInt(), betKindId };

            //  Central
            if (betKindId == BetKind.Central_2D18LoLive.ToInt())
                return new List<int> { BetKind.Central_2D18Lo.ToInt(), betKindId };

            if (betKindId == BetKind.Central_LoXien.ToInt())
                return new List<int> { BetKind.Central_Xien2.ToInt(), BetKind.Central_Xien3.ToInt(), BetKind.Central_Xien4.ToInt() };

            if (betKindId == BetKind.Central_Mixed_LoXien.ToInt())
                return new List<int> { BetKind.Central_Mixed_Xien2.ToInt(), BetKind.Central_Mixed_Xien3.ToInt(), BetKind.Central_Mixed_Xien4.ToInt() };

            if (betKindId == BetKind.Central_2DDau.ToInt())
                return new List<int> { BetKind.Central_2DDuoi.ToInt(), betKindId };

            if (betKindId == BetKind.Central_2DDuoi.ToInt())
                return new List<int> { BetKind.Central_2DDau.ToInt(), betKindId };

            //  Southern
            if (betKindId == BetKind.Southern_2D18LoLive.ToInt())
                return new List<int> { BetKind.Southern_2D18Lo.ToInt(), betKindId };

            if (betKindId == BetKind.Southern_LoXien.ToInt())
                return new List<int> { BetKind.Southern_Xien2.ToInt(), BetKind.Southern_Xien3.ToInt(), BetKind.Southern_Xien4.ToInt() };

            if (betKindId == BetKind.Southern_Mixed_LoXien.ToInt())
                return new List<int> { BetKind.Southern_Mixed_Xien2.ToInt(), BetKind.Southern_Mixed_Xien3.ToInt(), BetKind.Southern_Mixed_Xien4.ToInt() };

            if (betKindId == BetKind.Southern_2DDau.ToInt())
                return new List<int> { BetKind.Southern_2DDuoi.ToInt(), betKindId };

            if (betKindId == BetKind.Southern_2DDuoi.ToInt())
                return new List<int> { BetKind.Southern_2DDau.ToInt(), betKindId };

            return new List<int> { betKindId };
        }

        public static int GetNoOfNumbers(this int betKindId)
        {
            if (betKindId == BetKind.SecondNorthern_Northern_3DDau.ToInt() ||
                betKindId == BetKind.SecondNorthern_Northern_3DDuoi.ToInt() ||
                betKindId == BetKind.SecondNorthern_Northern_3D23Lo.ToInt() ||

                betKindId == BetKind.Central_3DDau.ToInt() ||
                betKindId == BetKind.Central_3DDuoi.ToInt() ||
                betKindId == BetKind.Central_3D17Lo.ToInt() ||
                betKindId == BetKind.Central_3D7Lo.ToInt() ||

                betKindId == BetKind.Southern_3DDau.ToInt() ||
                betKindId == BetKind.Southern_3DDuoi.ToInt() ||
                betKindId == BetKind.Southern_3D17Lo.ToInt() ||
                betKindId == BetKind.Southern_3D7Lo.ToInt())
                return 1000;

            if (betKindId == BetKind.SecondNorthern_Northern_4DDuoi.ToInt() ||
                betKindId == BetKind.SecondNorthern_Northern_4D20Lo.ToInt() ||

                betKindId == BetKind.Central_4DDuoi.ToInt() ||
                betKindId == BetKind.Central_4D16Lo.ToInt() ||

                betKindId == BetKind.Southern_4DDuoi.ToInt() ||
                betKindId == BetKind.Southern_4D16Lo.ToInt())
                return 10000;

            return 100;
        }

        public static int GetPositionOfPrize(this int prize, int position)
        {
            return 10 * prize + position;
        }

        public static int GetStartOfPosition(this int regionId)
        {
            return regionId == Region.Northern.ToInt() ? 2.GetPositionOfPrize(0) : 1.GetPositionOfPrize(0);
        }

        public static List<int> OutsTicketState()
        {
            return new List<int> { TicketState.Running.ToInt(), TicketState.Waiting.ToInt() };
        }

        public static List<int> RecalculateTicketState()
        {
            return new List<int> { TicketState.Completed.ToInt(), TicketState.Won.ToInt(), TicketState.Draw.ToInt(), TicketState.Lose.ToInt() };
        }

        public static List<int> CompletedTicketState()
        {
            return new List<int> { TicketState.Completed.ToInt(), TicketState.Refund.ToInt(), TicketState.Reject.ToInt(), TicketState.Won.ToInt(), TicketState.Draw.ToInt(), TicketState.Lose.ToInt() };
        }

        public static List<int> CompletedTicketWithoutRefundOrRejectState()
        {
            return new List<int> { TicketState.Completed.ToInt(), TicketState.Won.ToInt(), TicketState.Draw.ToInt(), TicketState.Lose.ToInt() };
        }

        public static List<int> RefundRejectTicketState()
        {
            return new List<int> { TicketState.Refund.ToInt(), TicketState.Reject.ToInt() };
        }

        public static List<int> AllTicketState()
        {
            return new List<int> { TicketState.Completed.ToInt(), TicketState.Refund.ToInt(), TicketState.Reject.ToInt(), TicketState.Won.ToInt(), TicketState.Draw.ToInt(), TicketState.Lose.ToInt(), TicketState.Running.ToInt(), TicketState.Waiting.ToInt() };
        }
    }
}
