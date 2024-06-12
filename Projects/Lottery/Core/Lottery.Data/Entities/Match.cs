using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Matches")]
    [Index(nameof(MatchCode), IsUnique = true)]
    [Index(nameof(KickOffTime))]
    public class Match : BaseEntity
    {
        [Key]
        public long MatchId { get; set; }

        [Required, MaxLength(50)]
        public string MatchCode { get; set; }

        [Required]
        public DateTime KickOffTime { get; set; }

        [Required]
        public int MatchState { get; set; }

        public virtual ICollection<MatchResult> MatchResults { get; set; }
    }
}
