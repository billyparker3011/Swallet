using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentPositionTakingModel
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public int BetKindId { get; set; }
        public string BetKindName { get; set; }
        public decimal PositionTaking { get; set; }
        public decimal DefaultPositionTaking { get; set; }
    }

    public class UpdateCasinoAgentPositionTakingModel
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public int BetKindId { get; set; }
        public decimal PositionTaking { get; set; }
        public decimal DefaultPositionTaking { get; set; }
    }
}
