using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Lottery.Core.UnitOfWorks;
using Lottery.Data;
using Lottery.Tools.AdjustOddsService.Services.InternalInitial;

namespace Lottery.Tools.AdjustOddsService.Helpers
{
    public static class WebApplicationBuilderHelper
    {
        public static void BuildInternalAdjustOddsService(this WebApplicationBuilder builder)
        {
            builder.AddSqlServer<LotteryContext>();
            builder.Services.AddScoped<ILotteryUow, LotteryUow>();
            builder.Services.AddHostedService<InternalInitialService>();
        }
    }
}
