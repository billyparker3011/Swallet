using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("CockFightAgentPostionTakings")]
    public class CockFightAgentPostionTaking : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long AgentId { get; set; }
        [Required]
        public long BetKindId { get; set; }
        [Required, Precision(18, 3)]
        public decimal PositionTaking { get; set; }
        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual CockFightBetKind CockFightBetKind { get; set; }
    }
}
