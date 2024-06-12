using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Players")]
    [Index(nameof(AgentId))]
    [Index(nameof(MasterId))]
    [Index(nameof(SupermasterId))]
    [Index(nameof(Username), IsUnique = true)]
    public class Player : BaseEntity
    {
        [Key]
        public long PlayerId { get; set; }

        [Required]
        public long AgentId { get; set; }

        [Required]
        public long MasterId { get; set; }

        [Required]
        public long SupermasterId { get; set; }

        [Required, MaxLength(150)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [MaxLength(500)]
        public string FirstName { get; set; }

        [MaxLength(500)]
        public string LastName { get; set; }

        [Required, Precision(18, 3)]
        public decimal Credit { get; set; }

        [Required]
        public int State { get; set; }

        public int? ParentState { get; set; }

        public DateTime? LatestChangePassword { get; set; }

        [Required, DefaultValue(false)]
        public bool Lock { get; set; }

        public virtual PlayerSession PlayerSession { get; set; }

        public virtual ICollection<PlayerAudit> PlayerAudits { get; set; }
    }
}
