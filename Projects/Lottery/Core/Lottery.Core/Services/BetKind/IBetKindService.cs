using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.BetKind;
using Lottery.Core.Models.BetKind;

namespace Lottery.Core.Services.BetKind
{
    public interface IBetKindService : IScopedDependency
    {
        GetFilterDataDto GetFilterDatas();
        Task<List<BetKindModel>> GetBetKinds(int? regionId, int? categoryId);
        Task UpdateBetKinds(List<BetKindModel> updatedItems);
    }
}
