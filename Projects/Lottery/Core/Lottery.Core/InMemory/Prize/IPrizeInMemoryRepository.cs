using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Prize;

namespace Lottery.Core.InMemory.Prize
{
    public interface IPrizeInMemoryRepository : IInMemoryRepository<int, PrizeModel>
    {
        List<PrizeModel> FindByRegion(int regionId);
    }
}
