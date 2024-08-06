using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Lottery.Data.Entities.Partners.CA
{
    [Table("CAPlayerBetSettingAgentHandicap")]
    public class CAPlayerBetSettingAgentHandicap
    {
        [Key, Column(Order = 0)]
        public long CAPlayerBetSettingId { get; set; }
        [Key, Column(Order = 1)]
        public long CAAgentHandicapId { get; set; }

        [ForeignKey(nameof(CAPlayerBetSettingId))]
        public virtual CAPlayerBetSetting CAPlayerBetSetting { get; set; }

        [ForeignKey(nameof(CAAgentHandicapId))]
        public virtual CAAgentHandicap CAAgentHandicap { get; set; }
    }
}
