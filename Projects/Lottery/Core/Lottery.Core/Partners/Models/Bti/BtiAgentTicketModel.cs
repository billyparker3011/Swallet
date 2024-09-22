using HnMicro.Framework.Responses;
namespace Lottery.Core.Partners.Models.Bti
{
    public class BtiAgentTicketModel
    {
        public List<BtiTicketModel> Items { get; set; }
        public ApiResponseMetadata Metadata { get; set; }

    }

    public class BtiTicketModel
    {
        public long PlayerId { get; set; }
        public string Username { get; set; }
    }

}
