namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands
{
    public class ChangePayoutOfPairNumbersCommand : AdjustOddsCommand
    {
        public Dictionary<int, List<string>> PairNumbers { get; set; }
    }
}
