namespace Lottery.Player.PlayerService.Requests.Player
{
    public class UpdatePlayerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? State { get; set; }
        public decimal? Credit { get; set; }
    }
}
