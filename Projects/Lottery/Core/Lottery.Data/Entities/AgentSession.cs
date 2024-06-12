using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("AgentSessions")]
    [Index(nameof(AgentId), IsUnique = true)]
    public class AgentSession : DefaultBaseEntityNoneAudit<long>
    {
        [Required]
        public long AgentId { get; set; }

        [MaxLength(10)]
        public string Hash { get; set; }

        [MaxLength(100)]
        public string IpAddress { get; set; }

        [MaxLength(100)]
        public string Platform { get; set; }

        [MaxLength(2000)]
        public string UserAgent { get; set; }

        [Required]
        public int State { get; set; }

        public DateTime? LatestDoingTime { get; set; }

        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }
    }
}
