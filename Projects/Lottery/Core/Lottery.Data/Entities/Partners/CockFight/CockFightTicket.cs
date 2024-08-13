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
        [Required]
        public Guid AccountId { get; set; }
        [MaxLength(255), Required]
        public string MemberRefId { get; set; }
        public decimal? AnteAmount { get; set; }
        [MaxLength(8), Required]
        public string ArenaCode { get; set; }
        public decimal? BetAmount { get; set; }
        [Required]
        public DateTime KickOffDate { get; set; }
        [MaxLength(3), Required]
        public string CurrencyCode { get; set; }
        [Required]
        public int FightNumber { get; set; }
        [MaxLength(64), Required]
        public string MatchDayCode { get; set; }
        public decimal Odds { get; set; }
        public int Result { get; set; } // CockFightTicketResult
        [MaxLength(32), Required]
        public string Selection { get; set; } // May be converted to enum
        public DateTime? SettledDateTime { get; set; }
        public Guid? Sid { get; set; }
        public int Status { get; set; } // CockFightTicketStatus
        public decimal? TicketAmount { get; set; }
        public decimal? WinlossAmount { get; set; }
        [MaxLength(255)]
        public string IpAddress { get; set; }
        [MaxLength(512)]
        public string UserAgent { get; set; }
    }
}
