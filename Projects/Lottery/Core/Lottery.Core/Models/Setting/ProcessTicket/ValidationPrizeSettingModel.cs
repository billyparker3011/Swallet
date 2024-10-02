using HnMicro.Core.Helpers;

namespace Lottery.Core.Models.Setting.ProcessTicket
{
    public class ValidationPrizeSettingModel
    {
        public int BetKindId { get; set; }
        public int Prize { get; set; }

        public static ValidationPrizeSettingModel CreateForBetKindEquals1()
        {
            return new ValidationPrizeSettingModel
            {
                BetKindId = Core.Enums.BetKind.FirstNorthern_Northern_De.ToInt(),
                Prize = 6
            };
        }

        public static ValidationPrizeSettingModel CreateForBetKindEquals10()
        {
            return new ValidationPrizeSettingModel
            {
                BetKindId = Core.Enums.BetKind.FirstNorthern_Northern_DeDau.ToInt(),
                Prize = 6
            };
        }
    }
}
