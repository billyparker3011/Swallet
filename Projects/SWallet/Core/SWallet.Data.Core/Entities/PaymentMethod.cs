using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("PaymentMethods")]
    public class PaymentMethod : DefaultBaseEntity<int>
    {
        [Required]
        public int PaymentPartner { get; set; }

        [Required, MaxLength(1000)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; }

        [MaxLength(500)]
        public string Icon { get; set; }

        [Required, Precision(18, 3)]
        public decimal Fee { get; set; }

        [Precision(18, 3)]
        public decimal? Min { get; set; }

        [Precision(18, 3)]
        public decimal? Max { get; set; }

        [Required, DefaultValue(true)]
        public bool Enabled { get; set; }
    }
}
