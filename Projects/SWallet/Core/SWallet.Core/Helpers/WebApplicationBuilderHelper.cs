using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SWallet.Data.Core;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Helpers
{
    public static class WebApplicationBuilderHelper
    {
        public static void BuildSWalletService(this WebApplicationBuilder builder)
        {
            builder.AddSqlServer<SWalletContext>();
            builder.Services.AddScoped<ISWalletUow, SWalletUow>();
        }
    }
}
