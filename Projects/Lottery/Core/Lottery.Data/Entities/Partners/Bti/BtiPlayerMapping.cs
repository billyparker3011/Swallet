using HnMicro.Framework.Data.Entities;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities.Partners.Bti
{
    [Table("BtiPlayerMappings")]
    [Index(nameof(CustomerId), nameof(CustomerLogin), IsUnique = true)]
    public class BtiPlayerMapping : DefaultBaseEntity<long>
    {
        [Required]
        public long PlayerId { get; set; }

        [Required, MaxLength(50)]
        public string CustomerId { get; set; }

        [Required, MaxLength(50)]
        public string CustomerLogin { get; set; }

        public long? BtiAgentId { get; set; }

        [MaxLength(255)]
        public string City { get; set; }

        [MaxLength(255)]
        public string Country { get; set; }

        [MaxLength(3)]
        public string CurrencyCode { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; }
    }
}
