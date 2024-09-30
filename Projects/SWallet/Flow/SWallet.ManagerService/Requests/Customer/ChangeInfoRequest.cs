namespace SWallet.ManagerService.Requests
{
    public class ChangeInfoRequest
    {
        public long CustomerId { get; set; } = 0L;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telegram { get; set; }
        public string Phone { get; set; }
        public bool? IsLock { get; set; }
        public int? State { get; set; }
    }
}
