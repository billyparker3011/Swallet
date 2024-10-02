using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Permissions")]
    public class Permission : BaseEntity
    {
        [Key]
        public int PermissionId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string PermissionName { get; set; }

        [Required]
        [MaxLength(250)]
        public string PermissionCode { get; set; }

        [Required]
        public int FeatureId { get; set; }

        [ForeignKey(nameof(FeatureId))]
        public virtual Feature Feature { get; set; }
    }
}
