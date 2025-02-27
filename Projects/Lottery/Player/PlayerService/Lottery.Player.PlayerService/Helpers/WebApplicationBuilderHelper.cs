﻿using Lottery.Player.PlayerService.Services.InternalInitial;

namespace Lottery.Player.PlayerService.Helpers
{
    public static class WebApplicationBuilderHelper
    {
        public static void BuildInternalPlayerService(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<InternalInitialService>();
        }
    }
}
