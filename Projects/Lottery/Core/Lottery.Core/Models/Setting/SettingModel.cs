using Lottery.Core.Enums;

namespace Lottery.Core.Models.Setting
{
    public class SettingModel
    {
        public int Id { get; set; }
        public CategoryOfSetting Category { get; set; }
        public string KeySetting { get; set; }
        public string ValueSetting { get; set; }
    }
}
