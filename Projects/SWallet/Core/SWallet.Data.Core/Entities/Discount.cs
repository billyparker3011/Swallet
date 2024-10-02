using HnMicro.Framework.Data.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Discounts")]
    public class Discount : BaseEntity
    {
        [Key]
        public int DiscountId { get; set; }

        [Required]
        [MaxLength(500)]
        public string DiscountName { get; set; }

        public string Description { get; set; }

        [Required]
        public bool IsStatic { get; set; }

        public int? SportKindId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Setting { get; set; }

        [Required, DefaultValue(true)]
        public bool IsEnabled { get; set; }
    }
}
