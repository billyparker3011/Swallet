namespace Lottery.Agent.AgentService.Requests.Audit
{
    public class GetAuditsByTypeRequest
    {
        public string SearchTerm {  get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
