using Lottery.Core.Enums.Partner;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAllBetPlayerModel : IBaseMessageModel
    {
        public long PlayerId { get; set; }
        public string NickName { get; set; }
        public bool IsAccountEnabled { get; set; }
        public bool IsAlowedToBet { get; set; }
        public string Player { get; set; }
        public string Agent { get; set; }
        public PartnerType Partner { get; set; } = PartnerType.Allbet;

        public string ToBodyJson()
        {
            var result = new
            {
                player = Player,
                agent = Agent,

            };
            return JsonConvert.SerializeObject(result);
        }
    }

    public class CasinoAllBetPlayerLoginModel : IBaseMessageModel
    {
        public long PlayerId { get; set; }
        public string Player { get; set; }
        public string TargetUrl { get; set; }
        public string Language { get; set; }
        public int? GameHall { get; set; }
        public string TableName { get; set; }
        public int? AppType { get; set; }
        public string ReturnUrl { get; set; }

        public PartnerType Partner { get; set; } = PartnerType.Allbet;

        public string ToBodyJson()
        {
            var result = new
            {
                player = Player,
                targetUrl = TargetUrl,
                language = Language,
                gameHall = GameHall,
                tableName = TableName,
                appType = AppType,
                returnUrl = ReturnUrl

            };
            return JsonConvert.SerializeObject(result);
        }
    }

    public class CasinoAllBetPlayerBetSettingModel : IBaseMessageModel
    {
        public long PlayerId { get; set; }
        [Required]
        public List<int> GeneralHandicapIds { get; set; }
        [Required]
        public int VipHandicapId { get; set; }
        public string Player { get; set; }
        public string Nickname { get; set; }
        public string[] GeneralHandicaps { get; set; }
        public string VipHandicap { get; set; }
        public bool? IsAccountEnabled { get; set; }
        public bool? IsAllowedToBet { get; set; }

        public PartnerType Partner { get; set; } = PartnerType.Allbet;

        public string ToBodyJson()
        {
            var result = new
            {
                player = Player,
                nickname = Nickname,
                generalHandicaps = GeneralHandicaps,
                vipHandicap = VipHandicap,
                isAccountEnabled = IsAccountEnabled,
                isAllowedToBet = IsAllowedToBet

            };
            return JsonConvert.SerializeObject(result);
        }
    }
}
