using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("BankAccounts")]
    [Index(nameof(BankId), nameof(NumberAccount), IsUnique = true)]
    public class BankAccount : BaseEntity
    {
        [Key]
        public int BankAccountId { get; set; }

        [Required]
        public int BankId { get; set; }

        [Required]
        [MaxLength(300)]
        public string NumberAccount { get; set; }

        [Required]
        [MaxLength(2000)]
        public string CardHolder { get; set; }

        [Required, DefaultValue(true)]
        public bool DepositEnabled { get; set; }

        [Required, DefaultValue(true)]
        public bool WithdrawEnabled { get; set; }

        [ForeignKey(nameof(BankId))]
        public virtual Bank Bank { get; set; }
    }
}
