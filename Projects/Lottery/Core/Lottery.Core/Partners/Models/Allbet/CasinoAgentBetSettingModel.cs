﻿using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentBetSettingModel
    {
        public CasinoAgentBetSettingModel()
        {

        }
        public CasinoAgentBetSettingModel(CasinoAgentBetSetting item)
        {
            Id = item.Id;
            AgentId = item.AgentId;
            BetKindId = item.BetKindId;
            DefaultVipHandicapId = item.DefaultVipHandicapId;
            DefaultGeneralHandicapIds = item.CasinoAgentBetSettingAgentHandicaps?.Select(x => x.CasinoAgentHandicap.Id).ToList();
            MinBet = item.MinBet;
            MaxBet = item.MaxBet;
            MaxWin = item.MaxWin;
            MaxLose = item.MaxLose;
        }
        public long Id { get; set; }
        public long AgentId { get; set; }
        public int BetKindId { get; set; }
        public int DefaultVipHandicapId { get; set; }
        public List<int> DefaultGeneralHandicapIds { get; set; }
        public decimal? MinBet { get; set; }
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
    }

    public class UpdateCasinoAgentBetSettingModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required]
        public List<int> DefaultGeneralHandicapIds { get; set; }
        [Required]
        public int DefaultVipHandicapId { get; set; }
        [Precision(18, 3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
    }

    public class CreateCasinoAgentBetSettingModel
    {
        [Required]
        public long AgentId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required]
        public List<int> DefaultGeneralHandicapIds { get; set; }
        [Required]
        public int DefaultVipHandicapId { get; set; }
        [Precision(18, 3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
    }
}
