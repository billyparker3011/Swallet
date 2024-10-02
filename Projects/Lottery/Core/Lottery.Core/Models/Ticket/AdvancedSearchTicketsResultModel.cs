using HnMicro.Framework.Responses;

namespace Lottery.Core.Models.Ticket
{
    public class AdvancedSearchTicketsResultModel
    {
        public List<TicketDetailModel> Items { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
