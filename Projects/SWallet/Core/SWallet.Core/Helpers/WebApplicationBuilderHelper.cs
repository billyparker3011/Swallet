using Microsoft.AspNetCore.Builder;

namespace SWallet.Core.Helpers
{
    public static class WebApplicationBuilderHelper
    {
        public static void BuildSWalletService(this WebApplicationBuilder builder)
        {
            //builder.AddSqlServer<SWalletContext>();
            //builder.Services.AddScoped<ISWalletUow, SWalletUow>();
        }
    }
}
