using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoService : IScopedDependency
    {
        Task AllBetPlayerLoginAsync(CasinoAllBetPlayerLoginModel model);
        Task CreateAllBetPlayerAsync(CasinoAllBetPlayerModel model);
        Task UpdateAllBetPlayerBetSettingAsync(CasinoAllBetPlayerBetSettingModel model);
        Task<string> GetGameUrlAsync();
        Task<string> GetCheckGameUrlAsync(long check);
    }
}
