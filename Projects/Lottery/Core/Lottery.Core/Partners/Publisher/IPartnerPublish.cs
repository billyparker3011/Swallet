using Lottery.Core.Partners.Models;

namespace Lottery.Core.Partners.Publish
{
    public interface IPartnerPublish
    {
        Task Publish(IBaseMessageModel model);
    }
}
