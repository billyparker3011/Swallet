using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.InMemory.Odds
{
    public interface IDefaultOddsInMemoryRepository : IInMemoryRepository<long, OddsModel>
    {
    }
}
