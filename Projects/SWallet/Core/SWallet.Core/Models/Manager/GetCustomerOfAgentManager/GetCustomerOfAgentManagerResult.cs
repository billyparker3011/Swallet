using HnMicro.Framework.Responses;
using SWallet.Core.Models;

namespace SWallet.Core.Models
{
    public class GetCustomerOfAgentManagerResult
    {
        public IEnumerable<CustomerModel> Customers { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
