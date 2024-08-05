﻿using Lottery.Core.Enums.Partner;

namespace Lottery.Core.Partners.Models.Tests
{
    public class Ga28LoginPlayerModel : IBaseMessageModel
    {
        public string MemberRefId { get; set; }
        public string AccountId { get; set; }
        public PartnerType Partner { get; set; } = PartnerType.GA28;
    }
}
