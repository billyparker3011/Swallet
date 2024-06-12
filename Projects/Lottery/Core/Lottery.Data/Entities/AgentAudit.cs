using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("AgentAudits")]
    public class AgentAudit : DefaultBaseEntityNoneAudit<long>
    {
        [Required]
        public long AgentId { get; set; }

        [MaxLength(100)]
        public string IpAddress { get; set; }

        [MaxLength(100)]
        public string Platform { get; set; }

        [MaxLength(2000)]
        public string UserAgent { get; set; }

        [MaxLength(4000)]
        public string Headers { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }
    }
}
