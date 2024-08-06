using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CAPlayerMapping
    {
        [Required]
        public long PlayerId { get; set; }
        [Required, MaxLength(50)]
        public string BookiePlayerId { get; set; }
        [Required, MaxLength(500)]
        public string NickName { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAccountEnable { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAlowedToBet { get; set; }

    }
}
