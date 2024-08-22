namespace Lottery.Core.Dtos.CockFight
{
    public class CockFightPlayerTicketDetailDto : CockFightPlayerBaseTicket
    {
        public long PlayerId { get; set; }
        public string Username { get; set; }
    }
}
