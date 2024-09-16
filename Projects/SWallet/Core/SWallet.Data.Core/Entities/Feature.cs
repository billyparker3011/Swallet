using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Features")]
    public class Feature : BaseEntity
    {
        [Key]
        public int FeatureId { get; set; }

        [Required]
        [MaxLength(500)]
        public string FeatureCode { get; set; }

        [Required]
        [MaxLength(2000)]
        public string FeatureName { get; set; }

        [Required]
        public bool Enabled { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
