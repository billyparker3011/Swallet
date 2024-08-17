﻿using Newtonsoft.Json;

namespace Lottery.Player.PlayerService.Requests.CockFight
{
    public class TransferTicketRequest
    {
        public Guid Id { get; set; }
        public string Amount { get; set; }
        [JsonProperty("member_ref_id")]
        public string MemberRefId { get; set; }
        public int Type { get; set; }
        public string Jackpot { get; set; }
        public TransferTicketDetailRequest Ticket { get; set; }
    }
}