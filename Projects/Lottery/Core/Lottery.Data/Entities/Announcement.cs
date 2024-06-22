using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Announcements")]
    public class Announcement : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Level { get; set; }
        [MaxLength(2000)]
        public string Content { get; set; }
    }
}
