using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Managers")]
    [Index(nameof(Username), nameof(ManagerCode), IsUnique = true)]
    [Index(nameof(SupermasterId))]
    [Index(nameof(MasterId))]
    [Index(nameof(RoleId))]
    public class Manager : BaseEntity
    {
        [Key]
        public long ManagerId { get; set; }

        [Required, DefaultValue(0L)]
        public long ParentId { get; set; }

        [Required]
        public int ManagerRole { get; set; }

        [MaxLength(250)]
        public string ManagerCode { get; set; }

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
