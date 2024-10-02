using HnMicro.Core.Scopes;
using Lottery.Core.Models.Prize;

namespace Lottery.Core.Services.Prizes
{
    public interface IPrizeService : IScopedDependency
    {
        PrizeFilterOptionModel GetFilterOptions();
        Task<List<PrizeModel>> GetPrizes(int? regionId);
        Task UpdatePrizes(UpdatePrizesModel model);
    }
}
