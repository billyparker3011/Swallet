namespace Lottery.Player.OddsService.Hubs.Behaviors
{
    //  All method Server calls to Client
    public interface IServerBehavior
    {
        Task Pong(string message);
        Task Odds(string message);
        Task LiveOdds(string message);
        Task UpdateOdds(string message);
        Task StartLive(string message);
        Task UpdateMatch(string message);
    }
}
