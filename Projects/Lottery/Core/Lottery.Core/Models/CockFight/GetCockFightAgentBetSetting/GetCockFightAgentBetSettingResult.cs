﻿namespace Lottery.Core.Models.CockFight.GetCockFightAgentBetSetting
{
    public class GetCockFightAgentBetSettingResult
    {
        public long BetKindId { get; set; }
        public string BetKindName { get; set; }
        public decimal MainLimitAmountPerFight { get; set; }
        public decimal DefaultMaxMainLimitAmountPerFight { get; set; }
        public decimal DrawLimitAmountPerFight { get; set; }
        public decimal DefaultMaxDrawLimitAmountPerFight { get; set; }
        public decimal LimitNumTicketPerFight { get; set; }
        public decimal DefaultMaxLimitNumTicketPerFight { get; set; }
    }
}