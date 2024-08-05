using Lottery.Core.Partners.Models;

namespace Lottery.Core.Partners.Periodic
{
    public interface IPeriodicService
    {
        Task Start();
        void Enqueue(IBaseMessageModel message);
        void Enqueue(string message);
    }
}
