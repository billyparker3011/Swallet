using Lottery.Player.PlayerService.Services.UpdatePlayerBetSetting;

namespace Lottery.Player.PlayerService.Services.InternalInitial
{
    public class InternalInitialService : IHostedService
    {
        private readonly IUpdatePlayerBetSettingService _updatePlayerBetSettingService;

        public InternalInitialService(IUpdatePlayerBetSettingService updatePlayerBetSettingService)
        {
            _updatePlayerBetSettingService = updatePlayerBetSettingService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Console.WriteLine($"Firing at {DateTime.Now}.");

            //using var peoridicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            //while (await peoridicTimer.WaitForNextTickAsync())
            //{
            //    Console.WriteLine($"Firing at {DateTime.Now}.");

            //    //var delay = _random.Next(20);
            //    await Task.Delay(TimeSpan.FromSeconds(8));
            //    Console.WriteLine($"Finish firing at {DateTime.Now}. Delay {8} (s).");
            //}
            await _updatePlayerBetSettingService.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _updatePlayerBetSettingService.Dispose();
        }
    }
}
