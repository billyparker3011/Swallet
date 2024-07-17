using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Lottery.Core.UnitOfWorks;
using Lottery.Data;
using Lottery.Player.PlayerService.Services.InternalInitial;

namespace Lottery.Player.PlayerService.Helpers
{
    public static class WebApplicationBuilderHelper
    {
        public static void BuildInternalPlayerService(this WebApplicationBuilder builder)
        {
            builder.AddSqlServer<LotteryContext>();
            builder.Services.AddScoped<ILotteryUow, LotteryUow>();
            builder.Services.AddHostedService<InternalInitialService>();
        }
    }
}
