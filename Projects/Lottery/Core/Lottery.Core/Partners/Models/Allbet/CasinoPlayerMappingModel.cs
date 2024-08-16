using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoPlayerMappingModel
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public string BookiePlayerId { get; set; }
        public string NickName { get; set; }
        public bool IsAccountEnable { get; set; }
        public bool IsAlowedToBet { get; set; }
    }

    public class CreateCasinoPlayerMappingModel
    {
        [Required]
        public long PlayerId { get; set; }
        [Required, MaxLength(50)]
        public string BookiePlayerId { get; set; }
        [Required, MaxLength(500)]
        public string NickName { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAccountEnable { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAlowedToBet { get; set; }

    }

    public class UpdateCasinoPlayerMappingModel
    {
        public long Id { get; set; }
        [Required]
        public long PlayerId { get; set; }
        [Required, MaxLength(50)]
        public string BookiePlayerId { get; set; }
        [Required, MaxLength(500)]
        public string NickName { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAccountEnable { get; set; }
        [Required, DefaultValue(true)]
        public bool IsAlowedToBet { get; set; }

    }
}
