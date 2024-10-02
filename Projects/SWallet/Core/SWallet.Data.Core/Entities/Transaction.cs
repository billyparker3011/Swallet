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

        //  Begin: Deposit transaction
        public int? DepositPaymentPartnerId { get; set; }
        public int? DepositPaymentMethodId { get; set; }
        [MaxLength(2000)]
        public string DepositBankName { get; set; }
        [MaxLength(300)]
        public string DepositNumberAccount { get; set; }
        [MaxLength(2000)]
        public string DepositCardHolder { get; set; }
        [MaxLength(100)]
        public string DepositContent { get; set; }
        [MaxLength(2000)]
        public string DepositToBankName { get; set; }
        [MaxLength(300)]
        public string DepositToNumberAccount { get; set; }
        [MaxLength(2000)]
        public string DepositToCardHolder { get; set; }
        //  End: Deposit transaction

        //  Begin: Withdraw transaction
        public int? WithdrawPaymentPartnerId { get; set; }
        public int? WithdrawPaymentMethodId { get; set; }
        [MaxLength(2000)]
        public string WithdrawBankName { get; set; }
        [MaxLength(300)]
        public string WithdrawNumberAccount { get; set; }
        [MaxLength(2000)]
        public string WithdrawCardHolder { get; set; }
        [MaxLength(2000)]
        public string WithdrawToBankName { get; set; }
        [MaxLength(300)]
        public string WithdrawToNumberAccount { get; set; }
        [MaxLength(2000)]
        public string WithdrawToCardHolder { get; set; }
        //  End: Withdraw transaction

        //  Begin: Discount
        public int? DiscountId { get; set; }
        public Guid? ReferenceDiscountDetail { get; set; }
        public long? ReferenceTransactionId { get; set; }
        //  End: Discount

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
