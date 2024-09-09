using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Banks")]
    public class Bank : BaseEntity
    {
        [Key]
        public int BankId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Icon { get; set; }

        [Required]
        public bool DepositEnabled { get; set; }

        [Required]
        public bool WithdrawEnabled { get; set; }
    }
}
