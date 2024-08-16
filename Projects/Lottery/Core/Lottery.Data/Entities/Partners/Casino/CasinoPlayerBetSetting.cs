using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoPlayerBetSettings")]
    public class CasinoPlayerBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required]
        public int VipHandicapId { get; set; }
        [Precision(18, 3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; }

        [ForeignKey(nameof(BetKindId))]
        public virtual CasinoBetKind CasinoBetKind { get; set; }

        [ForeignKey(nameof(VipHandicapId))]
        public virtual CasinoAgentHandicap CasinoAgentHandicap { get; set; }

        public ICollection<CasinoPlayerBetSettingAgentHandicap> CasinoPlayerBetSettingAgentHandicaps { get; set; }
    }
}
