using SWallet.Core.Enums;

namespace SWallet.Core.Models.Manager
{
    public class ManagerModel
    {
        public long ManagerId { get; set; }
        public long ParentId { get; set; }
        public int ManagerRole { get; set; }
        public string ManagerCode { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public ManagerState State { get; set; }
        public long SupermasterId { get; set; }
        public long MasterId { get; set; }
        public int RoleId { get; set; }
    }
}
