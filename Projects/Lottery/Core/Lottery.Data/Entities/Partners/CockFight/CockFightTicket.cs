using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CockFight
{
    [Table("CockFightTickets")]
    [Index(nameof(PlayerId))]
    [Index(nameof(AgentId))]
    [Index(nameof(MasterId))]
    [Index(nameof(SupermasterId))]
    [Index(nameof(BetKindId))]
    [Index(nameof(Sid))]
    public class CockFightTicket : DefaultBaseEntity<long>
    {
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
        public long? ParentId { get; set; }
        [Precision(18, 3)]
        public decimal? AnteAmount { get; set; }
        [MaxLength(8), Required]
        public string ArenaCode { get; set; }
        [Precision(18, 3)]
        public decimal? BetAmount { get; set; }
        [MaxLength(3), Required]
        public string CurrencyCode { get; set; }
        [Required]
        public int FightNumber { get; set; }
        [MaxLength(64), Required]
        public string MatchDayCode { get; set; }
        [Precision(18, 3)]
        public decimal? Odds { get; set; }
        public int Result { get; set; } // CockFightTicketResult
        [MaxLength(32), Required]
        public string Selection { get; set; } // May be converted to enum
        public DateTime? SettledDateTime { get; set; }
        [MaxLength(100), Required]
        public string Sid { get; set; }
        [Required]
        public int Status { get; set; } // CockFightTicketStatus
        [Precision(18, 3)]
        public decimal? TicketAmount { get; set; }
        [Precision(18, 3)]
        public decimal? WinlossAmount { get; set; }
        [MaxLength(255)]
        public string IpAddress { get; set; }
        [MaxLength(512)]
        public string UserAgent { get; set; }
        [Required]
        public DateTime TicketCreatedDate { get; set; }
        public DateTime? TicketModifiedDate { get; set; }
        [Precision(18, 3)]
        public decimal? ValidStake { get; set; }
        [MaxLength(5)]
        public string OddType { get; set; }
        public bool? ShowMore { get; set; }
    }
}
