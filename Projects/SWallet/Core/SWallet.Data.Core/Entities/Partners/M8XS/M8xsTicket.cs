using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("M8xsTickets")]
    [Index(nameof(ParentId))]
    [Index(nameof(CustomerId))]
    [Index(nameof(AgentId))]
    [Index(nameof(MasterId))]
    [Index(nameof(SupermasterId))]
    [Index(nameof(BetKindId))]
    [Index(nameof(SportKindId))]
    public class M8xsTicket
    {
        [Key]
        public long TicketId { get; set; }

        public long? ParentId { get; set; }

        [Required]
        public long CustomerId { get; set; }

        [Required]
        public long AgentId { get; set; }

        [Required]
        public long MasterId { get; set; }

        [Required]
        public long SupermasterId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required]
        public int SportKindId { get; set; }

        [Required]
        public long MatchId { get; set; }

        [Required]
        public DateTime KickOffTime { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required]
        public int ChannelId { get; set; }

        [Required, MaxLength(4000)]
        public string ChoosenNumbers { get; set; }

        public bool? ShowMore { get; set; }

        [Precision(18, 3)]
        public decimal? RewardRate { get; set; }

        [Required, Precision(18, 3)]
        public decimal Stake { get; set; }

        [Precision(18, 3)]
        public decimal? CustomerOdds { get; set; }  //  It's NULL when each number has difference odds

        [Required, Precision(18, 3)]
        public decimal CustomerPayout { get; set; }

        [Required, Precision(18, 3)]
        public decimal CustomerWinLoss { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftCustomerWinLoss { get; set; }

        [Required]
        public int State { get; set; }

        [Required, DefaultValue(false)]
        public bool IsLive { get; set; }

        public int? Prize { get; set; }

        public int? Position { get; set; }

        public int? Times { get; set; }

        [MaxLength(255)]
        public string MixedTimes { get; set; }

        [MaxLength(100)]
        public string IpAddress { get; set; }

        [MaxLength(100)]
        public string Platform { get; set; }

        [MaxLength(1000)]
        public string UserAgent { get; set; }

        public Guid? CorrelationCode { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual M8xsTicket Parent { get; set; }
    }
}
