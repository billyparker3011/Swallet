using HnMicro.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("Settings")]
    [Index(nameof(KeySetting))]
    public class Setting : DefaultBaseEntity<int>
    {
        [Required]
        public int Category { get; set; }

        [Required]
        public string KeySetting { get; set; }

        [MaxLength(4000)]
        public string ValueSetting { get; set; }
    }
}
