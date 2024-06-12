using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("PlayerOdds")]
    public class PlayerOdd : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal Buy { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal MinBet { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal MaxBet { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal MaxPerNumber { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual BetKind BetKind { get; set; }
    }
}
