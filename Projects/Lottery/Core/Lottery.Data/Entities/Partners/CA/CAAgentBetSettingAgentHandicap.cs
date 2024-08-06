using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CA
{
    [Table("CAAgentBetSettingAgentHandicap")]
    public class CAAgentBetSettingAgentHandicap
    {
        [Key, Column(Order = 0)]
        public long CAAgentBetSettingId { get; set; }
        [Key, Column(Order = 1)]
        public long CAAgentHandicapId { get; set; }

        [ForeignKey(nameof(CAAgentBetSettingId))]
        public virtual CAAgentBetSetting CAAgentBetSetting { get; set; }

        [ForeignKey(nameof(CAAgentHandicapId))]
        public virtual CAAgentHandicap CAAgentHandicap { get; set; }
    }
}
