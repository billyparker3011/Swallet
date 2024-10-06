using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("CasinoCustomerBetSettings")]
    public class CasinoCustomerBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required]
        public int DefaultVipHandicapId { get; set; }

        [Precision(18, 3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual CasinoBetKind CasinoBetKind { get; set; }
    }
}
