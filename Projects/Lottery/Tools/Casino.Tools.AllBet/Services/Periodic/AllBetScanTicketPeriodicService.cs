using Lottery.Core.Partners.Periodic;
using Microsoft.Extensions.Logging;

namespace Casino.Tools.AllBet.Services.Periodic
{
    public class AllBetScanTicketPeriodicService : AbstractScanTicketPeriodicService<AllBetScanTicketPeriodicService>
    {
        public AllBetScanTicketPeriodicService(ILogger<AllBetScanTicketPeriodicService> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }
    }
}
