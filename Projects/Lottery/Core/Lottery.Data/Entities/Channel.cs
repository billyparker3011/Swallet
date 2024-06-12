using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Channels")]
    public class Channel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Name { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required, MaxLength(50)]
        public string DayOfWeeks { get; set; }
    }
}
