using HnMicro.Framework.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Casino
{
    [Table("CasinoGameTypes")]
    public class CasinoGameType : DefaultBaseEntity<int>
    {
        [Required]
        public string Name { get; set; }
        [Required, MaxLength(10)]
        public string TableCode { get; set; }
        [Required, MaxLength(10)]
        public string GameCode { get; set; }
    }
}
