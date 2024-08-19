using HnMicro.Core.Helpers;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Models.Bookie;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.Bookie
{
    public static class CovertToBookieSettingModel
    {
        public static BookieSettingModel ToBookieSettingModel(this BookieSetting bookieSetting)
        {
            return new BookieSettingModel
            {
                Id = bookieSetting.Id,
                BookieTypeId = bookieSetting.BookieTypeId.ToEnum<PartnerType>(),
                SettingValue = bookieSetting.SettingValue
            };
        }
    }
}
