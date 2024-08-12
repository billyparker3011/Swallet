using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CA
{
    [Table("CABetKind")]
    [Index(nameof(Code), IsUnique = true)]
    public class CABetKind: DefaultBaseEntity<long>
    {
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
