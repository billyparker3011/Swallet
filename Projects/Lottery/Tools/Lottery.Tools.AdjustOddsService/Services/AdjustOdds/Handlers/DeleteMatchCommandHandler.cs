using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Tools.AdjustOddsService.InMemory.Payouts;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Handlers
{
    public class DeleteMatchCommandHandler : AdjustOddsCommandHandler
    {
        public DeleteMatchCommandHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Command { get; set; } = nameof(DeleteMatchCommand);

        public override void Handler<DeleteMatchCommand>(DeleteMatchCommand command)
        {
            using var scope = ServiceProvider.CreateScope();
            var inMemoryUow = scope.ServiceProvider.GetService<IInMemoryUnitOfWork>();
            var payoutByBetKindAndNumberInMemoryRepository = inMemoryUow.GetRepository<IPayoutByBetKindAndNumberInMemoryRepository>();
            payoutByBetKindAndNumberInMemoryRepository.RemoveByMatchId(command.MatchId);
        }
    }
}
