using HnMicro.Framework.Data.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lottery.Data.Entities
{
    [Table("BookieSettings")]
    public class BookieSetting : DefaultBaseEntity<int>
    {
        [Required]
        public int BookieTypeId { get; set; }

        public string SettingValue { get; set; }
    }
}
