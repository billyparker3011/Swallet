using Lottery.Data.Entities;

namespace Lottery.Core.Dtos.Audit
{
    public class AuditDto
    {
        public long AuditId { get; set; }
        public int Type { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AuditData AuditData { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EditedBy { get; set; }
        public List<AuditSettingData> AuditSettingDatas { get; set; } = new List<AuditSettingData>();
    }
}
