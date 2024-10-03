using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoBookieSettingService : IScopedDependency
    {
        Task<AllbetBookieSettingValue> GetCasinoBookieSettingValueAsync();
        Task CreateCasinoBookieSettingValueAsync(AllbetBookieSettingValue model);
        Task UpdateCasinoBookieSettingValueAsync(int id, AllbetBookieSettingValue model);
    }
}
