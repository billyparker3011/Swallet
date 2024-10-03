namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentHandicapModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
    }
}
