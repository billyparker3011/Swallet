namespace Lottery.Core.Partners.Subscriber
{
    public interface IPartnerSubscribeService
    {
        Task Subscribe(string channelName);
        Task SubscribeBookieChannel(string bookieChannel);
        Task SubscribeGa28BetKindChannel(string betKindChannel);
    }
}
