using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Lottery.Core.Services.Backgrounds;
using Lottery.Core.Services.Initial;
using Lottery.Core.UnitOfWorks;
using Lottery.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery.Core.Helpers
{
    public static class WebApplicationBuilderHelper
    {
        public static void BuildLotteryService(this WebApplicationBuilder builder, bool initialService = true)
        {
            builder.AddSqlServer<LotteryContext>();
            builder.Services.AddScoped<ILotteryUow, LotteryUow>();
            if (initialService) builder.Services.AddHostedService<InitialService>();
        }

        public static void BuildBackgroundServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<ProcessPlayerWinloseBackgroundService>();
        }
    }
}
