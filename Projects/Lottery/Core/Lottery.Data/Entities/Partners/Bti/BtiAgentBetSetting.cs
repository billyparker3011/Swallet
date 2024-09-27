using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Bti
{
    [Table("BtiAgentBetSettings")]
    public class BtiAgentBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long AgentId { get; set; }
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
        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }
        [ForeignKey(nameof(BetKindId))]
        public virtual BtiBetKind BtiBetKind { get; set; }
    }
}
