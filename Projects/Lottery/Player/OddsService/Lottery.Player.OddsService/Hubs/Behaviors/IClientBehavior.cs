namespace Lottery.Player.OddsService.Hubs.Behaviors
{
    //  All method Client calls to Server
    public interface IClientBehavior
    {
        Task Ping();
    }
}
