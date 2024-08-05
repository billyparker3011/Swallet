using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("CockFightPlayerMappings")]
    public class CockFightPlayerMapping : BaseEntity
    {
        [Key]
        public long Id { get; set; }
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
    }
}
