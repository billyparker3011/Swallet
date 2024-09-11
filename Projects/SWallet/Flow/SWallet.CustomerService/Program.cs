using HnMicro.Framework.Helpers;
using SWallet.Core.Filters;
using SWallet.Core.Helpers;
using SWallet.CustomerService.Helpers;

namespace SWallet.CustomerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.BuildServices(typeof(CustomerServiceFilter));
            builder.BuildSWalletService();
            builder.BuildSWalletCustomerService();

            var app = builder.Build();

            app.Usage();
            app.Run();
        }
    }
}