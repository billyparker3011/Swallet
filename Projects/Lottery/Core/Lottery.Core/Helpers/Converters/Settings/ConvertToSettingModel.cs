using HnMicro.Core.Helpers;
using Lottery.Core.Enums;
using Lottery.Core.Models.Setting;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.Settings
{
    public static class ConvertToSettingModel
    {
        public static SettingModel ToSettingModel(this Setting setting)
        {
            return new SettingModel
            {
                Id = setting.Id,
                Category = setting.Category.ToEnum<CategoryOfSetting>(),
                KeySetting = setting.KeySetting,
                ValueSetting = setting.ValueSetting
            };
        }
    }
}
