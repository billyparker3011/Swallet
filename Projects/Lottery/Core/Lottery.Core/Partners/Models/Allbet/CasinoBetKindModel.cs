using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoBetKindModel
    {
        public long Id { get; set; }
        public long BookieId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? RegionId { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsLive { get; set; }
        public int? OrderInCategory { get; set; }
        public decimal? Award { get; set; }
        public bool? Enabled { get; set; }
    }

    public class CreateCasinoBetKindModel
    {
        [Required]
        public int BookieId { get; set; }
        [Required, MaxLength(500)]
        public string Name { get; set; }
        [Required, MaxLength(10)]
        public string Code { get; set; }
        public int RegionId { get; set; }
        public int CategoryId { get; set; }
        public bool? IsLive { get; set; }
        public int? OrderInCategory { get; set; }
        public decimal? Award { get; set; }
        public bool? Enabled { get; set; }
    }

    public class UpdateCasinoBetKindModel
    {
        public int Id { get; set; }
        [Required]
        public int BookieId { get; set; }
        [Required, MaxLength(500)]
        public string Name { get; set; }
        [Required, MaxLength(10)]
        public string Code { get; set; }
        public int RegionId { get; set; }
        public int CategoryId { get; set; }
        public bool? IsLive { get; set; }
        public int? OrderInCategory { get; set; }
        public decimal? Award { get; set; }
        public bool? Enabled { get; set; }
    }
}
