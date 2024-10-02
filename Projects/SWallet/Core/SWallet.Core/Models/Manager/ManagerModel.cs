using SWallet.Core.Enums;

namespace SWallet.Core.Models
{
    public class ManagerModel
    {
        public long ManagerId { get; set; }
        public long ParentId { get; set; }
        public int ManagerRole { get; set; }
        public string ManagerCode { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int State { get; set; }
        public long SupermasterId { get; set; }
        public long MasterId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
