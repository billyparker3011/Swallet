using HnMicro.Core.Scopes;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.Services.Odds
{
    public interface INumberService : IScopedDependency
    {
        Task AddSuspendedNumbers(AddSuspendedNumbersModel model);
        Task DeleteSuspendedNumber(DeleteSuspendedNumberModel model);
        Task<List<int>> GetSuspendedNumbersByMatchAndBetKind(long matchId, int betKindId);
    }
}
