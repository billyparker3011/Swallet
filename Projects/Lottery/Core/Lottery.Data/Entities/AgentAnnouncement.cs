using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("AgentAnnouncements")]
    [Index(nameof(AgentId))]
    [Index(nameof(AnnouncementId))]
    public class AgentAnnouncement : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long AgentId { get; set; }
        [Required]
        public long AnnouncementId { get; set; }
        [Required]
        public int AnnouncementType { get; set; }
    }
}
