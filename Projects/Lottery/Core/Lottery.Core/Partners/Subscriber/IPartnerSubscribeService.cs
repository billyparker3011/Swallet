namespace Lottery.Core.Partners.Subscriber
{
    public interface IPartnerSubscribeService
    {
        Task Subscribe(string channelName);
    }
}
