namespace Lottery.Player.OddsService.Services.Initial
{
    public interface IPingPongService
    {
        Task Ping(string connectionId);
    }
}
