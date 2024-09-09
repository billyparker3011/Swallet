using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Roles")]
    [Index(nameof(RoleCode), IsUnique = true)]
    public class Role : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(500)]
        public string RoleName { get; set; }

        [Required]
        [MaxLength(250)]
        public string RoleCode { get; set; }
    }
}
