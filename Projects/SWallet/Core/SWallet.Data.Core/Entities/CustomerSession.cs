using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("CustomerSessions")]
    [Index(nameof(CustomerId), IsUnique = true)]
    public class CustomerSession : DefaultBaseEntityNoneAudit<long>
    {
        [Required]
        public long CustomerId { get; set; }

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

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
