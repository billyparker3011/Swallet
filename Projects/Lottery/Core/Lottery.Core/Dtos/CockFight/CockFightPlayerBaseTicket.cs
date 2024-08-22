using Lottery.Core.Enums.Partner.CockFight;
using Lottery.Core.Partners.Enums;

namespace Lottery.Core.Dtos.CockFight
{
    public class CockFightPlayerBaseTicket
    {
        public long TicketId { get; set; }
        public int BetKindId { get; set; }
        public decimal BetAmount { get; set; }
        public int FightNumber { get; set; }
        public string MatchDayCode { get; set; }
        public decimal Odds { get; set; }
        public CockFightTicketResult Result { get; set; }
        public CockFightSelection Selection { get; set; }
        public CockFightTicketStatus Status { get; set; }
        public decimal TicketAmount { get; set; }
        public decimal WinlossAmount { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
