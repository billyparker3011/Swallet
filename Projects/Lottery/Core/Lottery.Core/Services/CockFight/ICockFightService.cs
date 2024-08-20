using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Models.CockFight.GetBalance;
using Lottery.Core.Partners.Models.Ga28;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightService : IScopedDependency
    {
        Task CreateCockFightPlayer();
        Task LoginCockFightPlayer();
        Task<LoginPlayerInformationDto> GetCockFightUrl();
        Task<GetCockFightPlayerBalanceResult> GetCockFightPlayerBalance(string memberRefId);
        Task TransferCockFightPlayerTickets(Ga28TransferTicketModel model);
    }
}
