using HnMicro.Framework.Data.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Managers")]
    public class Manager : BaseEntity
    {
        [Key]
        public long ManagerId { get; set; }

        [Required, DefaultValue(0L)]
        public long ParentId { get; set; }

        [Required]
        public int ManagerRole { get; set; }

        [Required]
        [MaxLength(300)]
        public string Username { get; set; }

        [Required]
        [MaxLength(300)]
        public string Password { get; set; }

        [Required]
        [MaxLength(2000)]
        public string FullName { get; set; }

        [Required]
        public int State { get; set; }

        [Required, DefaultValue(0L)]
        public long SupermasterId { get; set; }

        [Required, DefaultValue(0L)]
        public long MasterId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual ManagerSession ManagerSession { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}
