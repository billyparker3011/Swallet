using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Levels")]
    public class Level : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(250)]
        public string LevelCode { get; set; }

        [Required]
        [MaxLength(2000)]
        public string LevelName { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }
    }
}
