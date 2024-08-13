using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoAgentBetSettingAgentHandicaps")]
    [Index(nameof(CasinoAgentBetSettingId), nameof(CasinoAgentHandicapId), IsUnique = true)]
    public class CasinoAgentBetSettingAgentHandicap : DefaultBaseEntity<long>
    {
        public long CasinoAgentBetSettingId { get; set; }

        public int CasinoAgentHandicapId { get; set; }

        [ForeignKey(nameof(CasinoAgentBetSettingId))]
        public virtual CasinoAgentBetSetting CasinoAgentBetSetting { get; set; }

        [ForeignKey(nameof(CasinoAgentHandicapId))]
        public virtual CasinoAgentHandicap CasinoAgentHandicap { get; set; }
    }
}
