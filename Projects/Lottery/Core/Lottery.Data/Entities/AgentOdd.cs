using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("AgentOdds")]
    public class AgentOdd : DefaultBaseEntity<long>
    {
        [Required]
        public long AgentId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal Buy { get; set; }

        [Required, Precision(18, 3)]
        public decimal MinBuy { get; set; }

        [Required, Precision(18, 3)]
        public decimal MaxBuy { get; set; }

        [Required, Precision(18, 3)]
        public decimal MinBet { get; set; }

        [Required, Precision(18, 3)]
        public decimal MaxBet { get; set; }

        [Required, Precision(18, 3)]
        public decimal MaxPerNumber { get; set; }

        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual BetKind BetKind { get; set; }
    }
}
