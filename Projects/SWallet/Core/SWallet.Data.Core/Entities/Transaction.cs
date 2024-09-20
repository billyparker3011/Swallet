using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Transactions")]
    [Index(nameof(CustomerId))]
    public class Transaction : BaseEntity
    {
        [Key]
        public long TransactionId { get; set; }

        [MaxLength(2000)]
        public string TransactionName { get; set; }

        [Required]
        public long CustomerId { get; set; }

        [Required]
        public int TransactionType { get; set; }

        [Required, Precision(18, 3)]
        public decimal OriginAmount { get; set; }

        [Required, Precision(18, 3)]
        public decimal Amount { get; set; }

        [Required]
        public int TransactionState { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
