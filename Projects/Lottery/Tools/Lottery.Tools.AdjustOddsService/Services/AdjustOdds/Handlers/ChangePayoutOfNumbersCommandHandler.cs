using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Handlers
{
    public class ChangePayoutOfNumbersCommandHandler : AdjustOddsCommandHandler
    {
        public ChangePayoutOfNumbersCommandHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Command { get; set; } = nameof(ChangePayoutOfNumbersCommand);

        public override void Handler<ChangePayoutOfNumbersCommand>(ChangePayoutOfNumbersCommand command)
        {
            Console.WriteLine("Change Payout.");
        }
    }
}
