using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.Region
{
    public interface IRegionInMemoryRepository : IInMemoryRepository<Enums.Region, RegionModel>
    {
    }
}
