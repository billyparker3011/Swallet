using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoPlayerBetSettingModel
    {
        public CasinoPlayerBetSettingModel()
        {
        }
        public CasinoPlayerBetSettingModel(CasinoPlayerBetSetting item)
        {
            Id = item.Id;
            PlayerId = item.PlayerId;
            BetKindId = item.BetKindId;
            VipHandicapId = item.VipHandicapId;
            GeneralHandicapIds = item.CasinoPlayerBetSettingAgentHandicaps?.Select(x => x.CasinoAgentHandicap.Id).ToList();
            MinBet = item.MinBet;
            MaxBet = item.MaxBet;
            MaxWin = item.MaxWin;
            MaxLose = item.MaxLose;
        }
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public int BetKindId { get; set; }
        public int VipHandicapId { get; set; }
        public List<int> GeneralHandicapIds { get; set; }
        public decimal? MinBet { get; set; }
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
    }

    public class CreateCasinoPlayerBetSettingModel
    {
        [Required]
        public long PlayerId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required]
        public List<int> GeneralHandicapIds { get; set; }
        [Required]
        public int VipHandicapId { get; set; }
        [Precision(18, 3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
        [Required]
        public bool IsAllowedToBet { get; set; }
        [Required]
        public bool IsAccountEnable { get; set; }
    }

    public class UpdateCasinoPlayerBetSettingModel
    {
        public long Id { get; set; }
        [Required]
        public long PlayerId { get; set; }
        [Required]
        public int BetKindId { get; set; }
        [Required]
        public List<int> GeneralHandicapIds { get; set; }
        [Required]
        public int VipHandicapId { get; set; }
        [Precision(18, 3)]
        public decimal? MinBet { get; set; }
        [Precision(18, 3)]
        public decimal? MaxBet { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MaxLose { get; set; }
        public bool IsAllowedToBet { get; set; }
        public bool IsAccountEnabled { get; set; }
    }
}
