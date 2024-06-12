using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Audits")]
    [Index(nameof(Audit.UserName))]
    public class Audit
    {
        [Key]
        public long AuditId { get; set; }

        [Required]
        public int Type { get; set; }

        [MaxLength(500)]
        public string Action { get; set; }

        [MaxLength(250)]
        [Required]
        public string UserName { get; set; }
        [MaxLength(250)]
        public string FirstName { get; set; }
        [MaxLength(250)]
        public string LastName { get; set; }

        public AuditData AuditData { get; set; }
        [Required]
        public long SupermasterId { get; set; }
        [Required]
        public long MasterId { get; set; }
        [Required]
        public long AgentId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [MaxLength(255)]
        public string EdittedBy { get; set; }
    }

    public class AuditData
    {
        public string Domain { get; set; }
        public string Browser { get; set; }
        public string Ip { get; set; }
        public string Country { get; set; }
        public string Detail { get; set; }
        public string NotOldValueDetail { get; set; }
        public decimal? OldValue { get; set; }
        public decimal? NewValue { get; set;}
    }

    public class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.Property(e => e.AuditData).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<AuditData>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
