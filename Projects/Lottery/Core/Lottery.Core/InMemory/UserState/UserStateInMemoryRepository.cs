using HnMicro.Core.Helpers;
using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.UserState
{
    public class UserStateInMemoryRepository : InMemoryRepository<Enums.UserState, UserStateModel>, IUserStateInMemoryRepository
    {
        public UserStateInMemoryRepository()
        {
            var items = typeof(Enums.UserState).GetListEnumInformation<Enums.UserState>();
            foreach (var item in items)
                InternalTryAddOrUpdate(new UserStateModel
                {
                    Id = item.Value,
                    Code = item.Code
                });
        }

        protected override void InternalTryAddOrUpdate(UserStateModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(UserStateModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
