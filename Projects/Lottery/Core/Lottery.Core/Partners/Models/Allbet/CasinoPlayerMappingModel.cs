namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoPlayerMappingModel
    {
        public long PlayerId { get; set; }
        public string BookiePlayerId { get; set; }
        public string NickName { get; set; }
        public bool IsAccountEnable { get; set; }
        public bool IsAlowedToBet { get; set; }
    }
}
