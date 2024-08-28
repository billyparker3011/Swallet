using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoTicketBetDetail")]
    [Index(nameof(CasinoTicketId))]
    public class CasinoTicketBetDetail : DefaultBaseEntity<long>
    {
        [Required]
        public long BetNum { get; set; }
        [Required]
        public long GameRoundId { get; set; }
        [Required]
        public int Status { get; set; }
        [Required, Precision(18, 3)]
        public decimal BetAmount { get; set; }
        [Required, Precision(18, 3)]
        public decimal Deposit { get; set; }
        [Required]
        public int GameType { get; set; }
        [Required]
        public int BetType { get; set; }
        [Required]
        public int Commission { get; set; }
        [Required, Precision(18, 3)]
        public decimal ExchangeRate { get; set; }
        [MaxLength(500)]
        public string GameResult { get; set; }
        [MaxLength(500)]
        public string GameResult2 { get; set; }
        [Precision(18, 3)]
        public decimal? WinOrLossAmount { get; set; }
        [Precision(18, 3)]
        public decimal? ValidAmount { get; set; }
        [Required, MaxLength(50)]
        public string BetTime { get; set; }
        [Required, MaxLength(10)]
        public string TableName { get; set; }
        [Required]
        public long BetMethod { get; set; }
        [Required]
        public long AppType { get; set; }
        [Required,MaxLength(50)]
        public string GameRoundStartTime { get; set; }
        [MaxLength(50)]
        public string GameRoundEndTime { get; set; }
        [Required, MaxLength(20)]
        public string Ip { get; set; }

        [Required, DefaultValue(false)]
        public bool IsCancel { get; set; }

        [Required]
        public long CasinoTicketId { get; set; }

        [Required, Precision(18, 3)]
        public decimal AgentWinLoss { get; set; }
        [Required, Precision(18, 3)]
        public decimal MasterWinLoss { get; set; }
        [Required, Precision(18, 3)]
        public decimal SupermasterWinLoss { get; set; }
        [Required, Precision(18, 3)]
        public decimal CompanyWinLoss { get; set; }

        [ForeignKey(nameof(CasinoTicketId))]
        public virtual CasinoTicket CasinoTicket { get; set; }
    }
}
