using HnMicro.Core.Scopes;
using Lottery.Core.Models.Setting.ProcessTicket;

namespace Lottery.Core.Services.Setting
{
    public interface IProcessTicketSettingService : IScopedDependency
    {
        Task UpdateScanWaitingTicketSetting(ScanWaitingTicketSettingModel model);
        Task UpdateValidationPrizeSetting(ValidationPrizeSettingModel model);
    }
}
