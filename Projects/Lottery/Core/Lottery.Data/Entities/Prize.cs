using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Prizes")]
    [Index(nameof(PrizeId), nameof(RegionId), IsUnique = true)]
    public class Prize
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PrizeId { get; set; }

        [Required]
        public int RegionId { get; set; }

        [MaxLength(500), Required]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public int NoOfNumbers { get; set; }
    }
}
