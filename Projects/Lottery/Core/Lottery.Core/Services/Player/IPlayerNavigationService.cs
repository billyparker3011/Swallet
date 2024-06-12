using HnMicro.Core.Scopes;
using Lottery.Core.Models.Navigation;

namespace Lottery.Core.Services.Player
{
    public interface IPlayerNavigationService : IScopedDependency
    {
        Task<List<NavigationModel>> MyNavigation();
    }
}
