using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentPositionTakingModel
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public int BetKindId { get; set; }
        public decimal PositionTaking { get; set; }
    }

    public class CreateCasinoAgentPositionTakingModel
    {
        [Required]
        public long AgentId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal PositionTaking { get; set; }
    }

    public class UpdateCasinoAgentPositionTakingModel
    {
        public long Id { get; set; }
        [Required]
        public long AgentId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal PositionTaking { get; set; }
    }
}
