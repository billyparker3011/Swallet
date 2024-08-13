namespace Lottery.Core.Partners.Models.Allbet
{
    public class BetDetail
    {
        public int AppType { get; set; }
        public decimal BetAmount { get; set; }
        public int BetMethod { get; set; }
        public long BetNum { get; set; }
        //public DateTime BetTime { get; set; }
        public int BetType { get; set; }
        public decimal Commission { get; set; }
        public decimal Deposit { get; set; }
        public decimal ExchangeRate { get; set; }
        public long GameRoundId { get; set; }
        //public DateTime GameRoundStartTime { get; set; }
        public int GameType { get; set; }
        public string Ip { get; set; }
        public int Status { get; set; }
        public string TableName { get; set; }
    }
}
