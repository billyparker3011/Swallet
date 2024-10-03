using HnMicro.Core.Scopes;
using Lottery.Core.Enums;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.Navigation;

namespace Lottery.Core.Services.Player
{
    public interface IBuildNavigationService : IScopedDependency
    {
        List<Category> GetDisplayCategory();
        List<SubNavigationModel> GetChildrenHandler(Category category, List<BetKindModel> betKinds, MatchModel runningMatch);
    }
}
