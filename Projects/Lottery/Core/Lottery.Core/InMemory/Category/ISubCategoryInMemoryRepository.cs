using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.Category
{
    public interface ISubCategoryInMemoryRepository : IInMemoryRepository<Enums.SubCategory, SubCategoryModel>
    {
    }
}
