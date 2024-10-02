using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("DiscountDetails")]
    public class DiscountDetail : DefaultBaseEntity<long>
    {
        [Required]
        public int DiscountId { get; set; }

        [Required]
        public Guid ReferenceTransaction { get; set; }

        [Required]
        public long CustomerId { get; set; }

        [ForeignKey(nameof(DiscountId))]
        public virtual Discount Discount { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
