namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands
{
    public class ChangePayoutOfNumbersCommand : AdjustOddsCommand
    {
        public int BetKindId { get; set; }
        public List<int> Numbers { get; set; }
    }
}
