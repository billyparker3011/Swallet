using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("CockFightPlayerBetSettings")]
    public class CockFightPlayerBetSetting : BaseEntity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long PlayerId { get; set; }
        [Required]
        public long BetKindId { get; set; }
        [Required]
        public decimal MainLimitAmountPerFight { get; set; }
        [Required]
        public decimal DrawLimitAmountPerFight { get; set; }
        [Required]
        public decimal LimitNumTicketPerFight { get; set; }
    }
}
