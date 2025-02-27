﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Tickets")]
    [Index(nameof(ParentId))]
    [Index(nameof(PlayerId))]
    [Index(nameof(AgentId))]
    [Index(nameof(MasterId))]
    [Index(nameof(SupermasterId))]
    [Index(nameof(BetKindId))]
    [Index(nameof(SportKindId))]
    public class Ticket
    {
        [Key]
        public long TicketId { get; set; }

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
        public decimal? PlayerOdds { get; set; }  //  It's NULL when each number has difference odds

        [Required, Precision(18, 3)]
        public decimal PlayerPayout { get; set; }

        [Required, Precision(18, 3)]
        public decimal PlayerWinLoss { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftPlayerWinLoss { get; set; }

        //  Agent
        [Precision(18, 3)]
        public decimal? AgentOdds { get; set; }

        [Required, Precision(18, 3)]
        public decimal AgentPayout { get; set; }

        [Required, Precision(18, 3)]
        public decimal AgentWinLoss { get; set; }

        [Required, Precision(18, 3)]
        public decimal AgentCommission { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftAgentWinLoss { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftAgentCommission { get; set; }

        [Required, Precision(18, 3)]
        public decimal AgentPt { get; set; }

        //  Master
        [Precision(18, 3)]
        public decimal? MasterOdds { get; set; }

        [Required, Precision(18, 3)]
        public decimal MasterPayout { get; set; }

        [Required, Precision(18, 3)]
        public decimal MasterWinLoss { get; set; }

        [Required, Precision(18, 3)]
        public decimal MasterCommission { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftMasterWinLoss { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftMasterCommission { get; set; }

        [Required, Precision(18, 3)]
        public decimal MasterPt { get; set; }

        //  Supermaster
        [Precision(18, 3)]
        public decimal? SupermasterOdds { get; set; }

        [Required, Precision(18, 3)]
        public decimal SupermasterPayout { get; set; }

        [Required, Precision(18, 3)]
        public decimal SupermasterWinLoss { get; set; }

        [Required, Precision(18, 3)]
        public decimal SupermasterCommission { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftSupermasterWinLoss { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftSupermasterCommission { get; set; }

        [Required, Precision(18, 3)]
        public decimal SupermasterPt { get; set; }

        //  Company
        [Precision(18, 3)]
        public decimal? CompanyOdds { get; set; }

        [Required, Precision(18, 3)]
        public decimal CompanyPayout { get; set; }

        [Required, Precision(18, 3)]
        public decimal CompanyWinLoss { get; set; }

        [Required, Precision(18, 3), DefaultValue(0)]
        public decimal DraftCompanyWinLoss { get; set; }

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
        public virtual Ticket Parent { get; set; }
    }
}
