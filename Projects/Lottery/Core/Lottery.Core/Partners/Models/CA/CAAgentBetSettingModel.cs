using Lottery.Data.Entities.Partners.CA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CAAgentBetSetting
    {
        public long Id { get; set; }
        [Required]
        public long AgentId { get; set; }
        [Required]
        public long CABetKindId { get; set; }
        [Required]
        public long DefaultVipHandicapId { get; set; }
        [Precision(18, 3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
    }
}
