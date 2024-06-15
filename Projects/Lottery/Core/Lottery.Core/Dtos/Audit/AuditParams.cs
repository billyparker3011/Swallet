using Lottery.Data.Entities;

namespace Lottery.Core.Dtos.Audit
{
    public class AuditParams
    {
        public string EditedUsername { get; set; }
        public string AgentUserName {  get; set; } 
        public string AgentFirstName { get; set; }
        public string AgentLastName { get; set; }
        public int Type { get; set; }
        public string Action { get; set; }
        public string DetailMessage { get; set; }
        public long SupermasterId { get; set; }
        public long MasterId { get; set; }
        public long AgentId { get; set; }
        public decimal? OldValue { get; set; }
        public decimal? NewValue { get; set; }
        public List<AuditSettingData> AuditSettingDatas { get; set; } = new List<AuditSettingData>();
    }
}
