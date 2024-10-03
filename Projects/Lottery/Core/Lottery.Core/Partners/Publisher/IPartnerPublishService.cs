using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models;

namespace Lottery.Core.Partners.Publish
{
    public interface IPartnerPublishService : IScopedDependency
    {
        Task Publish(IBaseMessageModel model);
    }
}
