using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.Helpers.Converters.Settings;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.InMemory.Ticket;
using Lottery.Core.Models.Setting.ProcessTicket;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Options;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class ScanTicketService : HnMicroBaseService<ScanTicketService>, IScanTicketService, IDisposable
{
    private Timer _timer;
    private ScanTicketOption _scanTicketOption;
    private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
    private readonly List<TicketModel> _tickets = new();

    public ScanTicketService(ILogger<ScanTicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IInMemoryUnitOfWork inMemoryUnitOfWork) : base(logger, serviceProvider, configuration, clockService)
    {
        _scanTicketOption = configuration.GetSection(ScanTicketOption.AppSettingName).Get<ScanTicketOption>();
        _inMemoryUnitOfWork = inMemoryUnitOfWork;
    }

    public void Start()
    {
        _timer = new Timer(CallBack, null, _scanTicketOption.IntervalInMilliseconds, Timeout.Infinite);
    }

    private void CallBack(object state)
    {
        //  Stop Timer
        _timer.Change(Timeout.Infinite, Timeout.Infinite);

        InternalScanTickets();

        //  Start Timer again
        _timer.Change(_scanTicketOption.IntervalInMilliseconds, Timeout.Infinite);
    }

    private double GetAvg()
    {
        return Math.Ceiling(1d * (_scanTicketOption.TimeToAcceptOrRejectTicketInSeconds + _scanTicketOption.IntervalInMilliseconds / 1000) / 2);
    }

    private void InternalScanTickets()
    {
        try
        {
            var settingInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();
            var scanWaitingTicketSetting = settingInMemoryRepository.FindByKey(ScanWaitingTicketSettingModel.KeySetting);
            var setting = scanWaitingTicketSetting == null
                        ? ScanWaitingTicketSettingModel.CreateDefault()
                        : scanWaitingTicketSetting.ValueSetting.ToSettingValue<ScanWaitingTicketSettingModel>();
            if (!setting.NoneLive.AllowAccepted && !setting.Live.AllowAccepted) return;

            _tickets.Clear();

            var ticketInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ITicketInMemoryRepository>();
            _tickets.AddRange(ticketInMemoryRepository.GetTopSequenceTickets(GetAvg()));
            if (_tickets.Count == 0) return;

            var rootTicketIds = _tickets.Select(f => f.TicketId).ToList();

            var ticketIds = new List<long>();
            ticketIds.AddRange(rootTicketIds);
            ticketIds.AddRange(_tickets.SelectMany(f => f.Children));

            using var scope = ServiceProvider.CreateScope();
            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();
            var ticketRepository = lotteryUow.GetRepository<ITicketRepository>();
            var tickets = ticketRepository.FindQueryBy(f => ticketIds.Contains(f.TicketId)).ToList();

            var rootTickets = tickets.Where(f => !f.ParentId.HasValue).ToList();
            foreach (var item in rootTickets)
            {
                item.State = TicketState.Running.ToInt();
                item.UpdatedAt = ClockService.GetUtcNow();
                ticketRepository.Update(item);
            }

            foreach (var item in tickets)
            {
                if (rootTickets.Any(f => f.TicketId == item.TicketId) || !item.ParentId.HasValue) continue;
                var parentTicket = rootTickets.FirstOrDefault(f => f.TicketId == item.ParentId.Value);
                if (parentTicket == null) continue;

                item.State = TicketState.Running.ToInt();
                item.UpdatedAt = parentTicket.UpdatedAt;
                ticketRepository.Update(item);
            }

            lotteryUow.SaveChanges();

            ticketInMemoryRepository.DeleteByIds(rootTicketIds);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message, ex.StackTrace);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}