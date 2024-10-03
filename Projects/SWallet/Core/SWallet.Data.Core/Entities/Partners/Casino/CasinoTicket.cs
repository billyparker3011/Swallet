using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("CasinoTicket")]
    [Index(nameof(TransactionId), IsUnique = true)]
    [Index(nameof(BookiePlayerId))]
    public class CasinoTicket : DefaultBaseEntity<long>
    {
        [Required]
        public long TransactionId { get;set; }
        [Required, MaxLength(50)]
        public string BookiePlayerId { get; set; }
        [Required, Precision(18,3)]
        public Decimal Amount { get; set; }
        [Required, MaxLength(50)]
        public string Currency { get; set; }
        [MaxLength(500)]
        public string Reason { get; set; }
        public int Type { get; set; }
        [Required]
        public bool IsRetry { get; set; }
        [Required, DefaultValue(false)]
        public bool IsCancel { get; set; }
        public long? CancelTransactionId { get; set; }
        [MaxLength(500)]
        public string CancelReason { get; set; }
        public bool? IsRetryCancel { get; set; }


        [Required]
        public long CustomerId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required]
        public long AgentId { get; set; }
        [Required]
        public long MasterId { get; set; }
        [Required]
        public long SupermasterId { get; set; }
        [Required]
        public decimal WinlossAmountTotal { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
