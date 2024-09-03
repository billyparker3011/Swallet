using HnMicro.Framework.Responses;

namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentTicketModel
    {
        public List<CasinoBetTableTicketModel> Items { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
