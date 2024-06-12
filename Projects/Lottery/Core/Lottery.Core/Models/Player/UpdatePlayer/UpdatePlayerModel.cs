namespace Lottery.Core.Models.Player.UpdatePlayer
{
    public class UpdatePlayerModel
    {
        public long PlayerId { get; set; }
        public int? State { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? Credit { get; set; }
    }
}
