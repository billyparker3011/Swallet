using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoAgentPositionTakings")]
    public class CasinoAgentPositionTaking : DefaultBaseEntity<long>
    {
        [Required]
        public long AgentId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal PositionTaking { get; set; }

        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual CasinoBetKind CasinoBetKind { get; set; }
    }
}
