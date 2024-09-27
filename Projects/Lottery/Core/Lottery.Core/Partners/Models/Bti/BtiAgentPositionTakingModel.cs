using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiAgentPositionTakingModel 
    {
        public long Id { get; set; }
        [Required]
        public long AgentId { get; set; }

        [Required]
        public int BetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal PositionTaking { get; set; }
        public decimal DefaultPositionTaking { get; set; }
        public string BetKindName { get; set; }
    }

    public class BtiDefaultAgentPositionTakingModel
    {
        public BtiDefaultAgentPositionTakingModel(int betKindId, decimal defaultPositionTaking)
        {
            BetKindId = betKindId;
            DefaultPositionTaking = defaultPositionTaking;
        }
        public int BetKindId { get; set; }
        public decimal DefaultPositionTaking { get; set; }
    }
}
