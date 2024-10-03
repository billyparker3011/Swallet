using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiPlayerBetSettingModel
    {
        public long Id { get; set; }
        [Required]
        public long PlayerId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required, Precision(18, 3)]
        public decimal MinBet { get; set; }
        [Required, Precision(18, 3)]
        public decimal MaxBet { get; set; }
        [Required, Precision(18, 3)]
        public decimal MaxWin { get; set; }
        [Required, Precision(18, 3)]
        public decimal MaxLoss { get; set; }
        public bool IsSynchronized { get; set; }
    }
}
