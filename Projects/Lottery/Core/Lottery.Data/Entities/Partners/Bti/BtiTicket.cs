using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Bti
{
    [Table("BtiTickets")]
    [Index(nameof(PlayerId))]
    [Index(nameof(AgentId))]
    [Index(nameof(MasterId))]
    [Index(nameof(SupermasterId))]
    [Index(nameof(BetKindId))]
    public class BtiTicket : DefaultBaseEntity<long>
    {

        [Required, MaxLength(50)]
        public string CustomerId { get; set; }
        public long? ReserveId { get; set; }
        public long? RequestId { get; set; }
        public long? PurcahseId { get; set; }
        [DefaultValue(false)]
        public bool IsFreeBet { get; set; }
        public long? TransactionId { get; set; }

        [Precision(18, 3)]
        public decimal BalanceResponse { get; set; }
        public int StatusResponse { get; set; }
        public string RequestBody { get; set; }
        public int Type { get; set; }
        public long? ParentId { get; set; }



        [Required]
        public long PlayerId { get; set; }
        [Required]
        public long AgentId { get; set; }
        [Required]
        public long MasterId { get; set; }
        [Required]
        public long SupermasterId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Precision(18, 3)]
        public decimal? BetAmount { get; set; }
        [MaxLength(15), Required]
        public string CurrencyCode { get; set; }
        [Precision(18, 3)]
        public decimal? Odds { get; set; }
        [Required]
        public int Status { get; set; }
        [Precision(18, 3)]
        public decimal? TicketAmount { get; set; }
        [Precision(18, 3)]
        public decimal? WinlossAmount { get; set; }
        [MaxLength(255)]
        public string IpAddress { get; set; }
        [MaxLength(1024)]
        public string UserAgent { get; set; }
        [Required]
        public DateTime TicketCreatedDate { get; set; }
        public DateTime? TicketModifiedDate { get; set; }
        [Precision(18, 3)]
        public decimal? ValidStake { get; set; }
        public bool? ShowMore { get; set; }
        [Required, Precision(18, 3)]
        public decimal AgentWinLoss { get; set; }
        [Required, Precision(18, 3)]
        public decimal AgentPt { get; set; }
        [Required, Precision(18, 3)]
        public decimal MasterWinLoss { get; set; }
        [Required, Precision(18, 3)]
        public decimal MasterPt { get; set; }
        [Required, Precision(18, 3)]
        public decimal SupermasterWinLoss { get; set; }
        [Required, Precision(18, 3)]
        public decimal SupermasterPt { get; set; }
        [Required, Precision(18, 3)]
        public decimal CompanyWinLoss { get; set; }
        [MaxLength(10)]
        public string OddsType { get; set; }
    }
}
