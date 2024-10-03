using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Helpers
{
    public static class CasinoHelper
    {

        public static class TypeTransfer
        {
            public static int Bet = 10;
            public static int Settle = 20;
            public static int ManualSettle = 21;
            public static int TransferIn = 30;
            public static int TransferOut = 31;
            public static int EventSettle = 40;

            public static int[] BetDetails = new int[] { Bet, Settle, ManualSettle };
            public static int[] CreditTransfer = new int[] { TransferIn, TransferOut };
            public static int[] EventDetails = new int[] { EventSettle };
        }
    }
}
