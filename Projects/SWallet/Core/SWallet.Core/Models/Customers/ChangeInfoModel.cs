using SWallet.Core.Enums;

namespace SWallet.Core.Models.Customers
{
    public class ChangeInfoModel
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
