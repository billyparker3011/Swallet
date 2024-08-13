using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoBetKinds")]
    [Index(nameof(Code), IsUnique = true)]
    public class CasinoBetKind
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Name { get; set; }

        [Required, MaxLength(10)]
        public string Code { get; set; }

        public string RegionId { get; set; }
        public string CategoryId { get; set; }
        public bool? IsLive { get; set; }
        public int? OrderInCategory { get; set; }
        public decimal? Award { get; set; }
        public bool? Enabled { get; set; }
    }
}
