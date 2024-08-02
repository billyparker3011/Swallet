using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.BetKind;
using Lottery.Core.Models.BetKind;

namespace Lottery.Core.Services.BetKind
{
    public interface IBetKindService : IScopedDependency
    {
        GetFilterDataDto GetFilterDatas();
        Task<List<BetKindSettingModel>> GetBetKinds(int? regionId, int? categoryId);
        Task UpdateBetKinds(List<BetKindSettingModel> updatedItems);
        Enums.BetKind GetReplacedBetKind(Enums.BetKind betKind);
    }
}
