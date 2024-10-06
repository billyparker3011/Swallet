using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("M8xsAgentBetSettings")]
    public class M8xsCustomerBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal Buy { get; set; } 

        [Required, Precision(18, 3)]
        public decimal MinBet { get; set; }

        [Required, Precision(18, 3)]
        public decimal MaxBet { get; set; }

        [Required, Precision(18, 3)]
        public decimal MaxPerNumber { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual M8xsBetKind M8xsBetKind { get; set; }
    }
}
