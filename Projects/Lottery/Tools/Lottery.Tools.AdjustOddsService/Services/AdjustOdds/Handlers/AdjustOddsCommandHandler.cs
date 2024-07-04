using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Handlers
{
    public abstract class AdjustOddsCommandHandler : IAdjustOddsCommandHandler
    {
        protected IServiceProvider ServiceProvider;

        protected AdjustOddsCommandHandler(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public abstract string Command { get; set; }

        public abstract void Handler<T>(T command) where T : AdjustOddsCommand;
    }
}
