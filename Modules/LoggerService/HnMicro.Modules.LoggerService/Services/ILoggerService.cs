using HnMicro.Framework.Logger.Models;

namespace HnMicro.Modules.LoggerService.Services
{
    public interface ILoggerService
    {
        Task Debug(LogRequestModel message);
        Task Info(LogRequestModel message);
        Task Warning(LogRequestModel message);
        Task Error(LogRequestModel message);
    }
}
