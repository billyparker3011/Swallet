using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Handlers;
using System.Collections.Concurrent;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds
{
    public class OddsAdjustmentService : HnMicroBaseService<OddsAdjustmentService>, IOddsAdjustmentService, IDisposable
    {
        private const int _intervalInMilliseconds = 1000;
        private Timer _timer;
        private volatile bool _isProcess;
        private readonly ConcurrentQueue<AdjustOddsCommand> _queue = new();
        private readonly List<IAdjustOddsCommandHandler> _handlers = new();

        public OddsAdjustmentService(ILogger<OddsAdjustmentService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService) : base(logger, serviceProvider, configuration, clockService)
        {
            RegisterCommandHandlers();
            InitTimer();
        }

        private void RegisterCommandHandlers()
        {
            var types = typeof(IAdjustOddsCommandHandler).GetDerivedClass().ToList();
            foreach (var item in types) _handlers.Add(Activator.CreateInstance(item, ServiceProvider) as IAdjustOddsCommandHandler);
        }

        private void InitTimer()
        {
            _timer = new Timer(CallBack, null, _intervalInMilliseconds, Timeout.Infinite);
        }

        private void CallBack(object state)
        {
            //  Stop Timer
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            InternalProcess();

            //  Start Timer again
            _timer.Change(_intervalInMilliseconds, Timeout.Infinite);
        }

        private void InternalProcess()
        {
            if (_isProcess) return;

            try
            {
                _isProcess = true;
                while (_queue.TryDequeue(out AdjustOddsCommand command))
                {
                    var handler = _handlers.FirstOrDefault(f => f.Command == command.GetType().Name);
                    if (handler == null) continue;

                    handler.Handler(command);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{ex.Message} - {ex.StackTrace}");
            }
            finally
            {
                _isProcess = false;
            }
        }

        public void Enqueue(AdjustOddsCommand command)
        {
            _queue.Enqueue(command);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
