using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.CA
{
    [Table("CAAgentHandicap")]
    [Index(nameof(Name), IsUnique = true)]
    public class CAAgentHandicap : DefaultBaseEntity<long>
    {
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
