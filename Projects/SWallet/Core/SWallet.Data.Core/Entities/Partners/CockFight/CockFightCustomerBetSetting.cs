using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("CockFightCustomerBetSettings")]
    public class CockFightCustomerBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal MainLimitAmountPerFight { get; set; }
        [Required, Precision(18, 3)]
        public decimal DrawLimitAmountPerFight { get; set; }
        [Required, Precision(18, 3)]
        public decimal LimitNumTicketPerFight { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual CockFightBetKind CockFightBetKind { get; set; }
    }
}
