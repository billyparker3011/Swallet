using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Agents")]
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(ParentId))]
    [Index(nameof(SupermasterId))]
    [Index(nameof(MasterId))]
    public class Agent : BaseEntity
    {
        [Key]
        public long AgentId { get; set; }

        [Required, DefaultValue(0L)]
        public long ParentId { get; set; }

        [MaxLength(255), Required]
        public string Username { get; set; }

        [MaxLength(32), Required]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int State { get; set; }

        public int? ParentState { get; set; }

        [MaxLength(512)]
        public string FirstName { get; set; }

        [MaxLength(512)]
        public string LastName { get; set; }

        [Required, Precision(18, 3)]
        public decimal Credit { get; set; }

        [Precision(18, 3)]
        public decimal? MemberMaxCredit { get; set; }

        [MaxLength(32)]
        public string SecurityCode { get; set; }

        public DateTime? LatestChangePassword { get; set; }

        public DateTime? LatestChangeSecurityCode { get; set; }

        [Required, DefaultValue(false)]
        public bool Lock { get; set; }

        [MaxLength(255)]
        public string Permissions { get; set; }

        [Required, DefaultValue(0L)]
        public long SupermasterId { get; set; }

        [Required, DefaultValue(0L)]
        public long MasterId { get; set; }

        public virtual AgentSession AgentSession { get; set; }

        public virtual ICollection<AgentAudit> AgentAudits { get; set; }

        public virtual ICollection<AgentOdd> AgentOdds { get; set; }

        public virtual ICollection<AgentPositionTaking> AgentPositionTakings { get; set; }
        public virtual ICollection<CockFightAgentBetSetting> AgentCockFightBetSettings { get; set; }
        public virtual ICollection<CockFightAgentPostionTaking> AgentCockFightPostionTakings { get; set; }
    }
}
