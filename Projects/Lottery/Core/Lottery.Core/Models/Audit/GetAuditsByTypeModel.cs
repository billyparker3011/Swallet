namespace Lottery.Core.Models.Audit
{
    public class GetAuditsByTypeModel
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SearchTerm { get; set; }
        public int Type { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
