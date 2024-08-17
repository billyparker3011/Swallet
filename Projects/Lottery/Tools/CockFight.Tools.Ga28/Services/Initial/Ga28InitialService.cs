using HnMicro.Framework.Exceptions;
using Lottery.Core.Partners.Subscriber;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CockFight.Tools.Ga28.Services.Initial
{
    public class Ga28InitialService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Ga28InitialService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InternalSubscriber();
        }

        private async Task InternalSubscriber()
        {
            var partnerChannelService = _serviceProvider.GetService<IPartnerSubscribeService>() ?? throw new HnMicroException();
            await partnerChannelService.Subscribe(Lottery.Core.Partners.Configs.PartnerChannelConfigs.Ga28Channel);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
