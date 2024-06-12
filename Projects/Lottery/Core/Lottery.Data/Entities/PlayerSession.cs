using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("PlayerSessions")]
    [Index(nameof(PlayerId), IsUnique = true)]
    public class PlayerSession : DefaultBaseEntityNoneAudit<long>
    {
        [Required]
        public long PlayerId { get; set; }

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

        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; }
    }
}
