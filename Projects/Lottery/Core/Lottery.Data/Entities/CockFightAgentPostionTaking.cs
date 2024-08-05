using HnMicro.Framework.Data.Entities;
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
        [Required]
        public decimal PositionTaking { get; set; }
    }
}
