using HnMicro.Framework.Helpers;
using HnMicro.Modules.EntityFrameworkCore.Helpers;
using SWallet.Core.Filters;
using SWallet.Core.Helpers;
using SWallet.Data.Core;
using SWallet.ManagerService.Helpers;

namespace SWallet.ManagerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.BuildServices(typeof(ManagerServiceFilter));
            builder.BuildSWalletService();
            builder.BuildSWalletManagerService();

            var app = builder.Build();

            app.Migrate<SWalletContext>();
            app.Usage();
            app.Run();
        }
    }
}