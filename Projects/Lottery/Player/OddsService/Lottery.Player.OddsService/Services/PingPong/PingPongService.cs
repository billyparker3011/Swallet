using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Player.OddsService.Configs;
using Lottery.Player.OddsService.Hubs;
using Lottery.Player.OddsService.InMemory.UserOnline;
using Microsoft.AspNetCore.SignalR;

namespace Lottery.Player.OddsService.Services.Initial
{
    public class PingPongService : IPingPongService
    {
        private Timer _timer;
        private readonly ILogger<PingPongService> _logger;
        private readonly IHubContext<OddsHub, IOddsHubBehavior> _hubContext;
        private readonly IClockService _clockService;
        private readonly IInMemoryUnitOfWork _unitOfWork;

        public PingPongService(ILogger<PingPongService> logger, IHubContext<OddsHub, IOddsHubBehavior> hubContext, IClockService clockService, IInMemoryUnitOfWork unitOfWork)
        {
            _logger = logger;
            _hubContext = hubContext;
            _clockService = clockService;
            _unitOfWork = unitOfWork;

            InitTimer();
        }

        private void InitTimer()
        {
            _timer = new Timer(CallBack, null, PingPongConfig.IntervalPingPongInMilliseconds, Timeout.Infinite);
        }

        private void CallBack(object state)
        {
            //  Stop Timer
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            InternalPong();

            //  Start Timer again
            _timer.Change(PingPongConfig.IntervalPingPongInMilliseconds, Timeout.Infinite);
        }

        private void InternalPong()
        {
            try
            {
                var userOnlineRepository = _unitOfWork.GetRepository<IUserOnlineInMemoryRepository>();
                var connectionIds = userOnlineRepository.FindAvailableUsers(PingPongConfig.IntervalPongInSeconds).Select(f => f.ConnectionId).ToList();
                if (connectionIds.Count == 0) return;

                Task.Run(() =>
                {
                    _hubContext.Clients.Clients(connectionIds).Pong("Pong");
                })
                .ContinueWith(action =>
                {
                    UpdatePongTime(connectionIds);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
            }
        }

        private void UpdatePongTime(List<string> connectionIds)
        {
            var userOnlineRepository = _unitOfWork.GetRepository<IUserOnlineInMemoryRepository>();
            var onlineUsers = userOnlineRepository.FindBy(f => connectionIds.Contains(f.ConnectionId)).ToList();
            onlineUsers.ForEach(f =>
            {
                f.PongTime = _clockService.GetUtcNow();
            });
        }

        public Task Ping(string connectionId)
        {
            var userOnlineRepository = _unitOfWork.GetRepository<IUserOnlineInMemoryRepository>();
            var userOnline = userOnlineRepository.FindById(connectionId);
            if (userOnline != null) userOnline.PingTime = _clockService.GetUtcNow();
            return Task.CompletedTask;
        }
    }
}
