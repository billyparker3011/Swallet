using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lottery.Data.Entities.Partners.Bti
{
    [Table("BtiPlayerBetSettings")]
    public class BtiPlayerBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required, Precision(18, 3)]
        public decimal MinBet { get; set; }
        [Required, Precision(18, 3)]
        public decimal MaxBet { get; set; }
        [Required, Precision(18, 3)]
        public decimal MaxWin { get; set; }
        [Required, Precision(18, 3)]
        public decimal MaxLoss { get; set; }
        public bool IsSynchronized { get; set; }
        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; }
        [ForeignKey(nameof(BetKindId))]
        public virtual BtiBetKind BtiBetKind { get; set; }
    }
}
