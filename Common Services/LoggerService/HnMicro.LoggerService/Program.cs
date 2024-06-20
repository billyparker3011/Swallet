using HnMicro.Core.Helpers;
using HnMicro.LoggerService.Helpers;

namespace HnMicro.LoggerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.BuildLoggerService();

            var app = builder.Build();

            app.Migrate();
            app.UseHealthChecks(OtherHelper.HealthChecksPath);
            app.Run();
        }
    }
}