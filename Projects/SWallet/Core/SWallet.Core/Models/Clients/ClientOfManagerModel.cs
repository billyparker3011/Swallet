namespace SWallet.Core.Models.Clients
{
    public class ClientOfManagerModel : ClientModel
    {
        public long ManagerId { get; set; }
        public long ParentId { get; set; }
        public List<string> Permissions { get; set; }
        public long SupermasterId { get; set; }
        public long MasterId { get; set; }
    }
}
