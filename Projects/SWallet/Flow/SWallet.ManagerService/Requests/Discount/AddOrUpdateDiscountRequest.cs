namespace SWallet.ManagerService.Requests.Discount
{
    public class AddOrUpdateDiscountRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsStatic { get; set; }
        public bool IsEnabled { get; set; }
        public int? SportKindId { get; set; }
        public DateTimeOffset? StartedDate { get; set; }
        public DateTimeOffset? EndedDate { get; set; }
        public DiscountSettingRequest Setting { get; set; }
    }
}
