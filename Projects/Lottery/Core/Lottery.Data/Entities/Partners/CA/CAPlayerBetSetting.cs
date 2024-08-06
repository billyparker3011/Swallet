using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Lottery.Data.Entities.Partners.CA
{
    [Table("CAPlayerBetSetting")]
    public class CAPlayerBetSetting : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerMappingId { get; set; }
        [Required]
        public long CABetKindId { get; set; }
        [Required]
        public long VipHandicapId { get; set; }
        [Precision(18,3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }

        [ForeignKey(nameof(PlayerMappingId))]
        public virtual CAPlayerMapping Player { get; set; }

        [ForeignKey(nameof(CABetKindId))]
        public virtual CABetKind CABetKind { get; set; }

        [ForeignKey(nameof(VipHandicapId))]
        public virtual CAAgentHandicap CAAgentHandicap { get; set; }

        public ICollection<CAPlayerBetSettingAgentHandicap> CAPlayerBetSettingAgentHandicaps { get; set; }

    }
}
