using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.CA
{
    public class CABetKind
    {
        public long Id { get; set; }
        [Required]
        public long BookieId { get; set; }
        [Required, MaxLength(500)]
        public string Name { get; set; }
        [Required, MaxLength(10)]
        public string Code { get; set; }
        public string RegionId { get; set; }
        public string CategoryId { get; set; }
        public bool? IsLive { get; set; }
        public int? OrderInCategory { get; set; }
        public decimal? Award { get; set; }
        public bool? Enabled { get; set; }
    }
}
