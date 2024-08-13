using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoPlayerBetSettingAgentHandicaps")]
    [Index(nameof(CasinoPlayerBetSettingId), nameof(CasinoAgentHandicapId), IsUnique = true)]
    public class CasinoPlayerBetSettingAgentHandicap : DefaultBaseEntity<long>
    {
        public long CasinoPlayerBetSettingId { get; set; }
        public int CasinoAgentHandicapId { get; set; }

        [ForeignKey(nameof(CasinoPlayerBetSettingId))]
        public virtual CasinoPlayerBetSetting CasinoPlayerBetSetting { get; set; }

        [ForeignKey(nameof(CasinoAgentHandicapId))]
        public virtual CasinoAgentHandicap CasinoAgentHandicap { get; set; }
    }
}
