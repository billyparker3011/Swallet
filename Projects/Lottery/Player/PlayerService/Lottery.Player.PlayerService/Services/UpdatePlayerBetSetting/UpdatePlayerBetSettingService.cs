namespace Lottery.Player.PlayerService.Services.UpdatePlayerBetSetting
{
    public class UpdatePlayerBetSettingService : IUpdatePlayerBetSettingService
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdatePlayerBetSettingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Start()
        {
            //Console.WriteLine("Test");
        }

        public void Dispose()
        {

        }
    }
}
