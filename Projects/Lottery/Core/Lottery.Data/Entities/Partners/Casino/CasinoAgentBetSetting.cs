using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoAgentBetSettings")]
    public class CasinoAgentBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long AgentId { get; set; }

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

        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual CasinoBetKind BetKind { get; set; }

        [ForeignKey(nameof(DefaultVipHandicapId))]
        public virtual CasinoAgentHandicap DefaultVipHandicap { get; set; }
    }
}
