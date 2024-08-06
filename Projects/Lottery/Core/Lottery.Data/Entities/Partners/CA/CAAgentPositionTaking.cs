using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CA
{
    [Table("CAAgentPositionTaking")]
    public class CAAgentPositionTaking: DefaultBaseEntity<long>
    {
        [Required]
        public long AgentId { get; set; }

        [Required]
        public long CABetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal PositionTaking { get; set; }

        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }

        [ForeignKey(nameof(CABetKindId))]
        public virtual CABetKind CABetKind { get; set; }
    }
}
