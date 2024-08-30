using HnMicro.Framework.Responses;
using Lottery.Core.Dtos.CockFight;

namespace Lottery.Core.Models.CockFight.GetCockFightAgentTicket
{
    public class CockFightAgentTicketResult
    {
        public List<CockFightPlayerTicketDetailDto> Items { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
