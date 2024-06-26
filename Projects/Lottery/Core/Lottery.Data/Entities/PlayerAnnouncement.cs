using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("PlayerAnnouncements")]
    [Index(nameof(PlayerId))]
    [Index(nameof(AnnouncementId))]
    public class PlayerAnnouncement : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long PlayerId { get; set; }
        [Required]
        public long AnnouncementId { get; set; }
        [Required]
        public int AnnouncementType { get; set; }
    }
}
