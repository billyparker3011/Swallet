using HnMicro.Core.Helpers;
using HnMicro.Framework.Logger.Models;
using Microsoft.Extensions.Configuration;

namespace HnMicro.Modules.LoggerService.Providers
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly List<IProvider> _providers = new List<IProvider>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ProviderFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            InitProviders();
        }

        public void Enqueue(LogModel message)
        {
            foreach (var provider in _providers) provider.Enqueue(message);
        }

        private void InitProviders()
        {
            var allDirevedProviders = typeof(IProvider).GetDerivedClass();
            foreach (var provider in allDirevedProviders)
            {
                _providers.Add(Activator.CreateInstance(provider, _serviceProvider, _configuration) as IProvider);
            }
        }
    }
}
