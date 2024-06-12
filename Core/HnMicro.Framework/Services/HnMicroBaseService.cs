using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HnMicro.Framework.Services
{
    public class HnMicroBaseService<T>
    {
        protected ILogger<T> Logger;
        protected IServiceProvider ServiceProvider;
        protected IConfiguration Configuration;
        protected IClockService ClockService;

        public HnMicroBaseService(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            ClockService = clockService;
        }
    }
}
