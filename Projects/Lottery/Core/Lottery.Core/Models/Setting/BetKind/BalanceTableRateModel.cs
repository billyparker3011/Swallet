﻿namespace Lottery.Core.Models.Setting.BetKind
{
    public class BalanceTableRateModel
    {
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal RateValue { get; set; }
        public bool Applied { get; set; }
    }
}
