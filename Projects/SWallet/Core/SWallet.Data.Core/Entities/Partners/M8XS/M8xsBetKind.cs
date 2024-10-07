using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("M8xsBetKinds")]
    public class M8xsBetKind
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(500), Required]
        public string Name { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, DefaultValue(false)]
        public bool IsLive { get; set; }

        public int? ReplaceByIdWhenLive { get; set; }

        [Required]
        public int OrderInCategory { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal Award { get; set; }

        [Required, DefaultValue(false)]
        public bool Enabled { get; set; }

        [Required, DefaultValue(false)]
        public bool IsMixed { get; set; }

        [MaxLength(15)]
        public string CorrelationBetKindIds { get; set; }   //  BetKindId1,BetKindId2
    }
}
