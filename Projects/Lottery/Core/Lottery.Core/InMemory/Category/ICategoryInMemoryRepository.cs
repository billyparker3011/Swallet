using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.Category
{
    public interface ICategoryInMemoryRepository : IInMemoryRepository<Enums.Category, CategoryModel>
    {
    }
}
