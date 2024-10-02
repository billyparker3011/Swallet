using HnMicro.Framework.Data.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWallet.Data.Core.Entities
{
    [Table("Settings")]
    public class Setting : DefaultBaseEntity<int>
    {
        [Required, DefaultValue(4)]
        public int NumberOfMaskCharacters { get; set; }

        [Required, MaxLength(5)]
        public string MaskCharacter { get; set; }

        [Required, MaxLength(10)]
        public string CurrencySymbol { get; set; }

        [Required, DefaultValue(0)]
        public int PaymentPartner { get; set; }

        [Required, DefaultValue(0)]
        public int DateTimeOffSet { get; set; }

        [MaxLength(500)]
        public string MainDomain { get; set; }
    }
}
