using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Bookie;

namespace Lottery.Core.InMemory.Bookies
{
    public interface IBookieSettingInMemoryRepository : IInMemoryRepository<int, BookieSettingModel>
    {
    }
}
