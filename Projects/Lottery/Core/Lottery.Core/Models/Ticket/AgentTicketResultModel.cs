using HnMicro.Framework.Responses;

namespace Lottery.Core.Models.Ticket
{
    public class AgentTicketResultModel
    {
        public List<TicketDetailModel> Items { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
