using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("CustomerBankAccounts")]
    [Index(nameof(CustomerId), nameof(BankId), nameof(NumberAccount), IsUnique = true)]
    public class CustomerBankAccount : DefaultBaseEntity<long>
    {
        [Required]
        public long CustomerId { get; set; }

        [Required]
        [MaxLength(250)]
        public string NumberAccount { get; set; }

        [Required]
        [MaxLength(2000)]
        public string CardHolder { get; set; }

        [Required]
        public int BankId { get; set; }

        [ForeignKey(nameof(BankId))]
        public virtual Bank Bank { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
