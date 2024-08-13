using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CockFight
{
    [Table("CockFightPlayerBetSettings")]
    public class CockFightPlayerBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal MainLimitAmountPerFight { get; set; }
        [Required, Precision(18, 3)]
        public decimal DrawLimitAmountPerFight { get; set; }
        [Required, Precision(18, 3)]
        public decimal LimitNumTicketPerFight { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual CockFightBetKind CockFightBetKind { get; set; }
    }
}
