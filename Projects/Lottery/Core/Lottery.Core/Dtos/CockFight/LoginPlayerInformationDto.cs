namespace Lottery.Core.Dtos.CockFight
{
    public class LoginPlayerInformationDto
    {
        public string Token { get; set; }
        public string GameClientUrl { get; set; }
        public PlayerInfoDto PlayerInfo { get; set; }
    }

    public class PlayerInfoDto
    {
        public string MemberRefId { get; set; }
        public string DisplayName { get; set; }
        public string AccountId { get; set; }
        public bool Enabled { get; set; }
        public bool Freeze { get; set; }
        public decimal? MainLimitAmountPerFight { get; set; }
        public decimal? DrawLimitAmountPerFight { get; set; }
        public decimal? LimitNumbTicketPerFight { get; set; }
    }
}
