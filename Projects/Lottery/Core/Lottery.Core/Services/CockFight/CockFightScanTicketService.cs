using HnMicro.Framework.Options;
using HnMicro.Framework.Services;
using Lottery.Core.Partners.CockFight.GA28;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight;

public class CockFightScanTicketService : HnMicroBaseService<CockFightScanTicketService>, ICockFightScanTicketService
{
    protected ServiceOption ServiceOption;
    private readonly ICockFightGa28Service _cockFightGa28Service;
    private volatile bool _initial;
    private PeriodicTimer _timer;
    private CancellationTokenSource _cts;

    public CockFightScanTicketService(ILogger<CockFightScanTicketService> logger, 
        IServiceProvider serviceProvider, 
        IConfiguration configuration, 
        IClockService clockService, 
        ServiceOption serviceOption,
        ICockFightGa28Service cockFightGa28Service) : base(logger, serviceProvider, configuration, clockService)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        ServiceOption = serviceOption;
        _cockFightGa28Service = cockFightGa28Service;
        Init();
    }

    private void Init()
    {
        if (_initial) return;

        try
        {
            _cts = new CancellationTokenSource();
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        }
        finally
        {
            _initial = true;
        }
    }

    public async Task Start()
    {
        Logger.LogInformation($"{ServiceOption.Name} - Started.");

        while (await _timer.WaitForNextTickAsync(_cts.Token))
        {
            await _cockFightGa28Service.ScanTickets();
        }
    }
}