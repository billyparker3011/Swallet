using HnMicro.Framework.Logger.Models;

namespace HnMicro.Modules.LoggerService.Providers
{
    public interface IProvider : IDisposable
    {
        void Enqueue(LogModel message);
    }
}
