using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("BalanceCustomers")]
    [Index(nameof(CustomerId))]
    public class BalanceCustomer : DefaultBaseEntity<long>
    {
        [Required]
        public long CustomerId { get; set; }

        [Required, Precision(18, 3)]
        public decimal Balance { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal TotalWinlose { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal HistoryTotalWinlose { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal TotalOutstanding { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
