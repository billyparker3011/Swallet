using Lottery.Core.Partners.Periodic;
using Microsoft.Extensions.Logging;

namespace CockFight.Tools.Ga28.Services.Periodic
{
    public class Ga28ScanTicketPeriodicService : AbstractScanTicketPeriodicService<Ga28ScanTicketPeriodicService>
    {
        public Ga28ScanTicketPeriodicService(ILogger<Ga28ScanTicketPeriodicService> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }
    }
}
