using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("MatchResults")]
    [Index(nameof(MatchId), nameof(RegionId), nameof(ChannelId))]
    public class MatchResult : DefaultBaseEntity<long>
    {
        [Required]
        public long MatchId { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required]
        public int ChannelId { get; set; }

        [Required, DefaultValue(false)]
        public bool IsLive { get; set; }

        [Required, DefaultValue(true)]
        public bool EnabledProcessTicket { get; set; }

        [Required, MaxLength(4000)]
        public string Results { get; set; }

        [ForeignKey(nameof(MatchId))]
        public virtual Match Match { get; set; }
    }
}
