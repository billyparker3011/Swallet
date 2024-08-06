using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CAAgentHandicaps
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, MaxLength(10)]
        public string Type { get; set; }
        [Required]
        public decimal MinBet { get; set; }
        [Required]
        public decimal MaxBet { get; set; }
    }
}
