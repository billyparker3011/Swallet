using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CA
{
    [Table("CAAgentBetSetting")]
    public class CAAgentBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long AgentId { get; set; }
        [Required]
        public long CABetKindId { get; set; }
        [Required]
        public long DefaultVipHandicapId { get; set; }
        [Precision(18,3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }

        [ForeignKey(nameof(AgentId))]
        public virtual Agent Agent { get; set; }

        [ForeignKey(nameof(CABetKindId))]
        public virtual CABetKind BetKind { get; set; }

        [ForeignKey(nameof(DefaultVipHandicapId))]
        public virtual CAAgentHandicap DefaultVipHandicap { get; set; }

        public ICollection<CAAgentBetSettingAgentHandicap> CAAgentBetSettingAgentHandicaps { get; set; }
    }
}
