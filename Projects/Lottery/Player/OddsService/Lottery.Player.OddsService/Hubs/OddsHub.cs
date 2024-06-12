using HnMicro.Framework.Configs;
using HnMicro.Framework.Exceptions;
using Lottery.Player.OddsService.Configs;
using Lottery.Player.OddsService.Models;
using Lottery.Player.OddsService.Services.Initial;
using Microsoft.AspNetCore.SignalR;

namespace Lottery.Player.OddsService.Hubs
{
    public class OddsHub : Hub<IOddsHubBehavior>
    {
        private readonly IOddsHubHandler _oddHubHandler;
        private readonly IPingPongService _pingPongService;

        public OddsHub(IOddsHubHandler oddHubHandler, IPingPongService pingPongService)
        {
            _oddHubHandler = oddHubHandler;
            _pingPongService = pingPongService;
        }

        public override async Task OnConnectedAsync()
        {
            var accessTokenQuery = Context.GetHttpContext().Request.Query.FirstOrDefault(f => f.Key == WebSocketConfigs.AccessToken);
            var betKindIdQuery = Context.GetHttpContext().Request.Query.FirstOrDefault(f => f.Key == QueryConfig.BetKindId);
            if (string.IsNullOrEmpty(accessTokenQuery.Key) || string.IsNullOrEmpty(betKindIdQuery.Key)) throw new ForbiddenException();

            var accessTokenValue = accessTokenQuery.Value.FirstOrDefault();
            var betKindIdValue = betKindIdQuery.Value.FirstOrDefault();
            if (string.IsNullOrEmpty(accessTokenValue) || string.IsNullOrEmpty(betKindIdValue)) throw new ForbiddenException();

            if (!int.TryParse(betKindIdValue, out var betKindId)) throw new ForbiddenException();

            await _oddHubHandler.CreateConnection(new ConnectionInformation
            {
                ConnectionId = Context.ConnectionId,
                AccessToken = accessTokenValue,
                BetKindId = betKindId
            });
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _oddHubHandler.DeleteConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Ping()
        {
            await _pingPongService.Ping(Context.ConnectionId);
        }
    }
}
