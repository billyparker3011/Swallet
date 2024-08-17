using HnMicro.Framework.Exceptions;
using Lottery.Core.Partners.Subscriber;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Casino.Tools.AllBet.Services.Initial
{
    public class AllbetInitialService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public AllbetInitialService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var partnerChannelService = _serviceProvider.GetService<IPartnerSubscribeService>() ?? throw new HnMicroException();
            await partnerChannelService.Subscribe(Lottery.Core.Partners.Configs.PartnerChannelConfigs.AllbetChannel);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
