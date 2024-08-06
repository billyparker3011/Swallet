using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CAAgentPositionTaking
    {
        public long Id { get; set; }
        [Required]
        public long AgentId { get; set; }

        [Required]
        public long CABetKindId { get; set; }

        [Required, Precision(18, 3)]
        public decimal PositionTaking { get; set; }
    }
}
