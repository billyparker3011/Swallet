using HnMicro.Framework.Data.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CockFight
{
    [Table("CockFightPlayerMappings")]
    public class CockFightPlayerMapping : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerId { get; set; }

        [Required]
        public bool IsInitial { get; set; }

        [MaxLength(255), Required]
        public string MemberRefId { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public bool IsFreeze { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        [Required, DefaultValue(false)]
        public bool NeedsRecalcBetSetting { get; set; }
    }
}
