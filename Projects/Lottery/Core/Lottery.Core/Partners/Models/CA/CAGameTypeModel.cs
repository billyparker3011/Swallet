using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CAGameType
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, MaxLength(10)]
        public string TableCode { get; set; }
        [Required, MaxLength(10)]
        public string GameCode { get; set; }
    }
}
