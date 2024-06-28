using HnMicro.Core.Scopes;
using Lottery.Core.Models.Setting.ProcessTicket;

namespace Lottery.Core.Services.Setting
{
    public interface IProcessTicketSettingService : IScopedDependency
    {
        Task<ScanWaitingTicketSettingModel> GetScanWaitingTicketSetting();
        Task UpdateScanWaitingTicketSetting(ScanWaitingTicketSettingModel model);

        Task<ValidationPrizeSettingModel> GetValidationPrizeSetting(int betKindId);
        Task UpdateValidationPrizeSetting(ValidationPrizeSettingModel model);
    }
}
